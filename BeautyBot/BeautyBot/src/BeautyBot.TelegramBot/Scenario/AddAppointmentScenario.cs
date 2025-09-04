using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Requests;

namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public class AddAppointmentScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly ISlotService _slotService;

        private readonly PostgreSqlProcedureRepository _procedureRepository;


        public AddAppointmentScenario(IUserService userService, IAppointmentService appointmentService, ISlotService slotService, PostgreSqlProcedureRepository procedureRepository)
        {
            _userService = userService;
            _appointmentService = appointmentService;

            _slotService = slotService;

            _procedureRepository = procedureRepository;
        }

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddAppointment;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
        {
            //верну выполненный сценарий если придёт какая-то левая инфа
            if (update.Message == null && update.CallbackQuery == null)
                return ScenarioResult.Completed;

            (Chat? currentChat, string? currentUserInput, int currentMessageId, BeautyBotUser? currentUser) = await Helper.HandleMessageAsyncGetData(update, _userService, ct);

            switch (context.CurrentStep)
            {
                case null:
                    return await HandleInitialStep(botClient, context, currentUser, currentChat, ct);

                case "BaseProcedure":
                    return await HandleBaseProcedureStep(botClient, context, currentChat, currentUserInput, ct);

                case "TypeProcedure":
                    return await HandleTypeProcedureStep(botClient, context, currentChat, currentUserInput, currentMessageId, ct);

                case "DateProcedure":
                    return await HandleChooseDateStep(botClient, context, currentChat, currentUserInput, currentMessageId, ct);

                case "ApproveDateProcedure":
                    return await HandleApproveDateStep(botClient, context, currentChat, currentUserInput, currentMessageId, ct);

                case "ChooseTimeProcedure":
                    return await HandleChooseTimeStep(botClient, context, currentChat, currentUserInput, ct);

                case "ApproveTimeProcedure":
                    return await HandleApproveTimeStep(botClient, context, currentChat, currentUserInput, ct);
                default:
                    await botClient.SendMessage(currentChat, "Неизвестный шаг сценария", replyMarkup: Keyboards.firstStep, cancellationToken: ct);
                    break;
            }
            return ScenarioResult.Completed;
        }

        private async Task<ScenarioResult> HandleInitialStep(ITelegramBotClient botClient, ScenarioContext context, BeautyBotUser user, Chat chat, CancellationToken ct)
        {
            context.Data["User"] = user;

            await botClient.SendMessage(chat, "Куда записываемся?", replyMarkup: Keyboards.secondStep, cancellationToken: ct);

            context.CurrentStep = "BaseProcedure";

            return ScenarioResult.Transition;
        }
        private async Task<ScenarioResult> HandleBaseProcedureStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            if (userInput == Constants.Back)
            {
                context.DataHistory.Pop();

                context.CurrentStep = null;

                return await HandleInitialStep(botClient, context, (BeautyBotUser)context.Data["User"], chat, ct);
            }


            if (userInput != Constants.Manicure && userInput != Constants.Pedicure)
                throw new Exception("Что-то пошло не так");

            context.Data["BaseProcedure"] = userInput;

            context.DataHistory.Push(userInput);

            switch (context.Data["BaseProcedure"])
            {
                case Constants.Manicure:
                    await botClient.SendMessage(chat, "Выберите маникюр", replyMarkup: Keyboards.thirdManicureStep, cancellationToken: ct);
                    break;
                case Constants.Pedicure:
                    await botClient.SendMessage(chat, "Выберите педикюр", replyMarkup: Keyboards.thirdPedicureStep, cancellationToken: ct);
                    break;
                default:
                    break;
            }

            context.CurrentStep = "TypeProcedure";

            return ScenarioResult.Transition;
        }
        private async Task<ScenarioResult> HandleTypeProcedureStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, int messageId, CancellationToken ct)
        {
            context.Data.TryGetValue("BaseProcedure", out var procedureType);

            if (procedureType == null)
                throw new Exception("Что-то пошло не так");

            if (userInput == Constants.Back)
            {
                context.DataHistory.Pop();

                context.CurrentStep = "BaseProcedure";

                await botClient.DeleteMessage(chatId: chat, messageId: messageId - 1, cancellationToken: ct );

                return await HandleBaseProcedureStep(botClient, context, chat, context.DataHistory.Peek(), ct);
            }

            context.DataHistory.Push(userInput);

            context.Data["TypeProcedure"] = ProcedureFactory.CreateProcedure(userInput, (string)procedureType);

            var calendarMarkup = DaySlotsKeyboard(
                DateTime.Today,
                DateTime.Today,
                DateTime.Today.AddDays(60),
                await _slotService.GetUnavailableSlotsByDate(ct)
                );

            await botClient.SendMessage(chat, "Выберите дату", replyMarkup: Keyboards.cancelOrBack, cancellationToken: ct);

            await botClient.SendMessage(chat, "✖ - означает, что на выбранную дату нет свободных слотов", replyMarkup: calendarMarkup, cancellationToken: ct);

            context.CurrentStep = "DateProcedure";

            return ScenarioResult.Transition;
        }
        private async Task<ScenarioResult> HandleChooseDateStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, int messageId, CancellationToken ct)
        {
            if (userInput == Constants.Back)
            { 
                context.CurrentStep = "TypeProcedure";

                return await HandleTypeProcedureStep(botClient, context, chat, context.DataHistory.Pop(), messageId, ct);
            }

            var date = Helper.ParseDateFromString(userInput);

            await botClient.SendMessage(chat, $"Выбранная дата - {date}\n\nВерно?", replyMarkup: Keyboards.approveDate, cancellationToken: ct);

            context.Data["DateProcedure"] = date;

            context.CurrentStep = "ApproveDateProcedure";

            return ScenarioResult.Transition;
        }
        private async Task<ScenarioResult> HandleApproveDateStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, int messageId, CancellationToken ct)
        {
            if (userInput != Constants.Accept)
            {
                var calendarMarkup = DaySlotsKeyboard(
                    DateTime.Today,
                    DateTime.Today,
                    DateTime.Today.AddDays(60),
                    await _slotService.GetUnavailableSlotsByDate(ct)
                    );

                await botClient.DeleteMessage(chatId: chat, messageId: messageId - 2, cancellationToken: ct);
                await botClient.DeleteMessage(chatId: chat, messageId: messageId - 1, cancellationToken: ct);

                await botClient.SendMessage(chat, "Выберите другую дату", replyMarkup: Keyboards.cancelOrBack, cancellationToken: ct);

                await botClient.SendMessage(chat, "✖ - означает, что на выбранную дату нет свободных слотов", replyMarkup: calendarMarkup, cancellationToken: ct);

                context.CurrentStep = "DateProcedure";

                context.Data["DateProcedure"] = null;

                return ScenarioResult.Transition;
            }

            if (!context.Data.TryGetValue("DateProcedure", out var dateObj))
                throw new KeyNotFoundException("Не найдена контекст даты");

            if (dateObj is not DateOnly date)
                throw new InvalidCastException($"Ожидался DateOnly, получен {dateObj?.GetType().Name ?? "null"}");

            var slots = await _slotService.GetSlotsByDate(date, ct);

            await botClient.SendMessage(chat, "Выберите время", replyMarkup: TimeSlotsKeyboard(slots), cancellationToken: ct);

            context.CurrentStep = "ChooseTimeProcedure";

            return ScenarioResult.Transition;
        }
        private async Task<ScenarioResult> HandleChooseTimeStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            if (!TimeOnly.TryParse(userInput, out var time))
                throw new InvalidCastException($"Ожидался TimeOnly, получен {time.GetType().Name ?? "null"}");

            await botClient.SendMessage(chat, $"Выбранное время - {time}\n\nВерно?", replyMarkup: Keyboards.approveTime, cancellationToken: ct);

            context.Data["Time"] = time;

            context.CurrentStep = "ApproveTimeProcedure";

            return ScenarioResult.Transition;
        }

        private async Task<ScenarioResult> HandleApproveTimeStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            if (userInput != Constants.Accept)
            {
                if (!context.Data.TryGetValue("DateProcedure", out var dateObj))
                    throw new KeyNotFoundException("Не найдена контекст даты");

                if (dateObj is not DateOnly date)
                    throw new InvalidCastException($"Ожидался DateOnly, получен {dateObj?.GetType().Name ?? "null"}");

                var slots = await _slotService.GetSlotsByDate(date, ct);

                await botClient.SendMessage(chat, "Выберите другое время", replyMarkup: TimeSlotsKeyboard(slots), cancellationToken: ct);

                context.Data["Time"] = null;

                context.CurrentStep = "ChooseTimeProcedure";

                return ScenarioResult.Transition;
            }

            await _procedureRepository.Add((IProcedure)context.Data["TypeProcedure"], ct);

            var newAppointment = await _appointmentService.AddAppointment(
                (BeautyBotUser)context.Data["User"],
                (IProcedure)context.Data["TypeProcedure"],
                (DateOnly)context.Data["DateProcedure"],
                (TimeOnly)context.Data["Time"],
                ct);

            await _slotService.UpdateSlotFromAppointment(newAppointment, ct);

            await botClient.SendMessage(
                chat, 
                $"Вы успешно записаны🤗\n\nЖдём Вас {context.Data["DateProcedure"]} в {context.Data["Time"]}\n\nПо адресу г. Екатеринбург ул. Ленина 1, офис 101\n\nПрекрасного дня ☀️", 
                replyMarkup: Keyboards.firstStep, 
                cancellationToken: ct);

            return ScenarioResult.Completed;
        }

        private ReplyKeyboardMarkup TimeSlotsKeyboard(IEnumerable<Slot> slots)
        {
            if (slots.Count() == 0)
                Console.WriteLine("На выбранную дату записей нет");
            
            var timeSlotButtons = slots
                .Select(button => new KeyboardButton(TimeOnly.FromDateTime(button.StartTime).ToString()))
                .Select(btn => new[] { btn })
                .Concat(Keyboards.cancelOrBack.Keyboard)
                .ToArray(); 

            return new ReplyKeyboardMarkup(timeSlotButtons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        private static InlineKeyboardMarkup DaySlotsKeyboard(
            DateTime displayMonth,
            DateTime minDate,
            DateTime maxDate,
            List<DateOnly> unavailableDays)
        {
            string PrevMonthCallback = "prev_month_";
            string NextMonthCallback = "next_month_";
            string DaySelectedCallback = "day_selected_";

            var keyboardButtons = new List<List<InlineKeyboardButton>>();

            // Add day names row
            var dayNamesRow = new List<InlineKeyboardButton>();
            for (int i = 0; i < 7; i++)
            {
                dayNamesRow.Add(InlineKeyboardButton.WithCallbackData(
                    CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(i + (int)DayOfWeek.Monday) % 7], // Start from Monday
                    "day_name_no_action" // No action for day names
                ));
            }
            keyboardButtons.Add(dayNamesRow);

            // Add month days
            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);

            // Calculate offset for the first day of the month (0 for Monday, 6 for Sunday)
            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

            var currentRow = new List<InlineKeyboardButton>();

            // Add empty buttons for the days before the first day of the month
            for (int i = 0; i < offset; i++)
                currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));

            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDay = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var currentDateOnly = DateOnly.FromDateTime(currentDay);
                // Check if the day is within the allowed range
                bool isDayValid = currentDay >= minDate && currentDay <= maxDate;
                bool isDayAvailable = isDayValid && !unavailableDays.Contains(currentDateOnly);

                if (isDayValid)
                {
                    currentRow.Add(InlineKeyboardButton.WithCallbackData(
                        isDayAvailable ? day.ToString() : "✖",
                        isDayAvailable ? $"{DaySelectedCallback}{currentDay:yyyy-MM-dd}" : "day_unavailable"
                    ));
                }
                else
                {
                    currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));
                }

                if (currentRow.Count == 7)
                {
                    keyboardButtons.Add(currentRow);
                    currentRow = new List<InlineKeyboardButton>();
                }
            }
            // Add remaining empty buttons for the last row
            if (currentRow.Any())
            {
                while (currentRow.Count < 7)
                    currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));

                keyboardButtons.Add(currentRow);
            }

            // Add navigation row
            var navigationRow = new List<InlineKeyboardButton>();

            // Previous month button (only if not the starting month)
            if (displayMonth.Year > minDate.Year || (displayMonth.Year == minDate.Year && displayMonth.Month > minDate.Month))
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                    "<",
                    $"{PrevMonthCallback}{displayMonth.AddMonths(-1):yyyy-MM-dd}"
                ));
            }
            else
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button")); // Placeholder for alignment
            }

            navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                displayMonth.ToString("MMMM yyyy", CultureInfo.CurrentCulture),
                "month_display_no_action"
            ));

            // Next month button (only if there are available days in the next month within the 60-day range)
            if (displayMonth.AddMonths(1) <= maxDate.AddDays(1).Date) // Check if next month potentially contains valid dates
            {
                // Check if any day in the next month falls within the maxDate range
                var nextMonthFirstDay = new DateTime(displayMonth.AddMonths(1).Year, displayMonth.AddMonths(1).Month, 1);
                if (nextMonthFirstDay <= maxDate)
                {
                    navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                        ">",
                        $"{NextMonthCallback}{displayMonth.AddMonths(1):yyyy-MM-dd}"
                    ));
                }
                else
                {
                    navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button")); // Placeholder for alignment
                }
            }
            else
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button")); // Placeholder for alignment
            }

            keyboardButtons.Add(navigationRow);

            return new InlineKeyboardMarkup(keyboardButtons);
        }


        private ReplyKeyboardMarkup TimeSlotsKeyboard1(Dictionary<TimeOnly, bool> slots)
        {


            // Создаем кнопки слотов времени (по одной в ряд)
            var timeSlotButtons = slots
                .Select(slot => new KeyboardButton(slot.Key.ToString("HH:mm")))
                .Select(btn => new[] { btn })
                .ToList();

            // Добавляем дополнительный ряд с кнопками навигации
            var navigationButtons = new[]
            {
                new KeyboardButton("Назад"),
                new KeyboardButton("Отмена")
            };

            // Объединяем все ряды кнопок
            timeSlotButtons.Add(navigationButtons);

            return new ReplyKeyboardMarkup(timeSlotButtons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

    }
}

