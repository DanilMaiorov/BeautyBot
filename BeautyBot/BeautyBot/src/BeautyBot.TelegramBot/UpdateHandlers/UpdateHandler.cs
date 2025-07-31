using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Globalization;
using System.Linq;
using BeautyBot.src.BeautyBot.Application.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.TelegramBot.UpdateHandlers
{
    public delegate void MessageEventHandler(string message);
    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly ISlotService _slotService;

        private readonly IProcedureCatalogService _procedureCatalogService;
        private readonly IPriceCalculationService _priceCalculationService;


        private readonly ICreateAppointmentService _createAppointmentService;

        private readonly CancellationToken _ct;

        //добавлю 2 события
        public event MessageEventHandler OnHandleUpdateStarted;
        public event MessageEventHandler OnHandleUpdateCompleted;

        public UpdateHandler(
            IUserService userService,
            IAppointmentService appointmentService,
            IProcedureCatalogService procedureCatalogService,
            IPriceCalculationService priceCalculationService,

            ISlotService slotService,
            ICreateAppointmentService createAppointmentService,
            CancellationToken ct)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _procedureCatalogService = procedureCatalogService ?? throw new ArgumentNullException(nameof(procedureCatalogService));
            _priceCalculationService = priceCalculationService ?? throw new ArgumentNullException(nameof(priceCalculationService));

            _slotService = slotService;
            _createAppointmentService = createAppointmentService;
            _ct = ct;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.Message != null)
            {
                await OnMessage(botClient, update, update.Message, ct);
            }
            else if (update.CallbackQuery != null)
            {
                await OnCallbackQuery(botClient, update, update.CallbackQuery, ct);
            }
            else
            {
                //await OnUnknown();
                return;
            }
        }


        private async Task OnMessage(ITelegramBotClient botClient, Update update, Message message, CancellationToken ct)
        {
            Chat currentChat = update.Message.Chat;
            long telegramCurrentUserId = update.Message.From.Id;
            string telegramCurrentUserName = update.Message.From.Username;
            string input = update.Message.Text;

            string eventMessage = "";

            try
            {
                var currentUser = await _userService.GetUser(telegramCurrentUserId, _ct);

                var currentUserTaskList = currentUser != null
                    ? await _appointmentService.GetUserAppointmentsByUserId(currentUser.UserId, _ct)
                    : null;


                if (currentUser == null)
                {
                    if (input != "/start" && input != "Старт")
                    {
                        await botClient.SendMessage(currentChat, "Для запуска бота необходимо нажать на кнопку ниже или ввести /start", replyMarkup: Keyboards.start, cancellationToken: _ct);
                        return;
                    }
                }

                //присваиваю начальное значение введёного сообщения
                eventMessage = input;

                //НАЧАЛО ОБРАБОТКИ СООБЩЕНИЯ
                OnHandleUpdateStarted?.Invoke(eventMessage);

                (string inputCommand, string inputText, string chooseMonth, DateOnly appointmentDate, TimeOnly appointmentTime, Guid taskGuid) = Helper.InputCheck(input, currentUserTaskList);

                input = inputCommand.Replace("/", string.Empty);

                //проверка списка с шагами записи
                var steps = await _createAppointmentService.GetSteps();

                //текущий шаг
                CreateAppointmentTemplate currentStep = await _createAppointmentService.GetStep();

                if (steps.Count < 0)
                    throw new ApplicationException("Недопустимое значение количество шагов");

                //первый шаг создания записи
                if (steps.Count == 1)
                {
                    ManicureType manicureType = ManicureType.None;
                    PedicureType pedicureType = PedicureType.None;
                     
                    switch (currentStep.Procedure)
                    {
                        case Pedicure:
                            pedicureType = Helper.GetEnumValueOrDefault(input, pedicureType);
                            break;
                        case Manicure:
                            manicureType = Helper.GetEnumValueOrDefault(input, manicureType);
                            break;
                        default:
                            break;
                    }

                    if (manicureType != ManicureType.None || pedicureType != PedicureType.None)
                    {
                        var procedure = ProcedureFactory.CreateProcedure(input, currentStep.Procedure);

                        await _createAppointmentService.AddStep(procedure);

                        var calendarMarkup = DaySlotsKeyboard(
                            DateTime.Today,
                            DateTime.Today,
                            DateTime.Today.AddDays(60),
                            await _slotService.GetUnavailableDaySlots(ct)
                            );


                        await botClient.SendMessage(currentChat, "Выберите дату", replyMarkup: Keyboards.cancelOrBack, cancellationToken: _ct);

                        await botClient.SendMessage(currentChat, "✖ - означает, что на выбранную дату нет свободных слотов", replyMarkup: calendarMarkup, cancellationToken: _ct);

                        return;
                    }
                }

                Console.WriteLine(input);

                //КОНЕЦ ОБРАБОТКИ СООБЩЕНИЯ
                OnHandleUpdateCompleted?.Invoke(eventMessage);

                //объявление типа данных команды
                Command command;

                if (Enum.TryParse<Command>(input, true, out var result))
                    command = result;
                else
                    command = default;

                Helper.GetEnumValueOrDefault(input, command);
                //обработка основных команд
                switch (command)
                {
                    case Command.Start:
                        if (currentUser == null)
                            currentUser = await _userService.RegisterUser(telegramCurrentUserId, telegramCurrentUserName, _ct);
                        
                        await botClient.SendMessage(currentChat, "Спасибо за регистрацию", replyMarkup: Keyboards.firstStep, cancellationToken: _ct);
                        //await Helper.CommandsRender(currentChat, botClient, _ct);
                        break;

                    case Command.Help:
                        await ShowHelp(botClient, currentChat, currentUser, _ct);
                        break;

                    case Command.Info:
                        await ShowInfo(botClient, currentChat, _ct);
                        break;

                    case Command.ShowActiveAppointments:
                        await ShowAppointments(currentUser.UserId, botClient, currentChat, _ct);
                        break;

                    case Command.ShowAllAppointments:
                        await ShowAppointments(currentUser.UserId, botClient, currentChat, _ct, true);
                        break;

                    //case Command.AddAppointment:
                    //    ProcedureFactory.CreateProcedure(inputText, out IProcedure procedure);
                    //    await _appointmentService.AddAppointment(currentUser, procedure, DateTime.Now, _ct);
                    //    break;

                    case Command.Del:
                        await _appointmentService.CancelAppointment(taskGuid, _ct);
                        break;

                    case Command.Add:
                        //ProcedureFactory.CreateProcedure(inputText, out IProcedure procedure);
                        //await _appointmentService.AddAppointment(currentUser, procedure, DateTime.Now, _ct);
                        break;

                    case Command.CancelAppointment:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("CancelAppointment");
                        break;

                    case Command.EditAppointment:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("EditAppointmentEditAppointment");
                        break;

                    case Command.UpdateAppointment:
                        await _appointmentService.UpdateAppointment(taskGuid, AppointmentState.Completed, _ct);
                        break;

                    case Command.FindAppointment:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("FindAppointmentFindAppointment");
                        break;

                    //команды создания записи
                    case Command.Create:
                        await botClient.SendMessage(currentChat, "Куда записываемся?", replyMarkup: Keyboards.secondStep, cancellationToken: _ct);
                        break;


                    case Command.Manicure:
                        await botClient.SendMessage(currentChat, "Выберите маникюр", replyMarkup: Keyboards.thirdManicureStep, cancellationToken: _ct);
                        await _createAppointmentService.AddStep(new Manicure("", default, default, default), appointmentDate);
                        break;


                    case Command.Pedicure:
                        await botClient.SendMessage(currentChat, "Выберите педикюр", replyMarkup: Keyboards.thirdPedicureStep, cancellationToken: _ct);
                        await _createAppointmentService.AddStep(new Pedicure("", default, default, default), appointmentDate);
                        break;

                    case Command.ChangeDate:
                        await HandleBackCommand(update, botClient, currentChat.Id, steps, currentStep, ct, "Выберите другую дату");
                        break;

                    case Command.ChangeTime:
                        await HandleBackCommand(update, botClient, currentChat.Id, steps, currentStep, ct, "Выберите другое время");

                        break;

                    case Command.Time:
                        await _createAppointmentService.AddStep(currentStep.Procedure, currentStep.AppointmentDate, appointmentTime);
                        await botClient.SendMessage(currentChat, $"Выбранное время - {appointmentTime}\n\nВерно?", replyMarkup: Keyboards.approveTime, cancellationToken: _ct);
                        break;

                    case Command.Back:
                        await HandleBackCommand(update, botClient, currentChat.Id, steps, currentStep, ct, "Выберите новую дату");
                        break;

                    case Command.Approve:

                        if (steps.Count == 4)
                        {
                            await _appointmentService.AddAppointment(
                                currentUser, 
                                currentStep.Procedure,
                                currentStep.AppointmentDate.ToDateTime(currentStep.AppointmentTime), 
                                _ct);

                            await _slotService.UpdateSlot(currentStep.AppointmentDate, currentStep.AppointmentTime, ct);

                            await botClient.SendMessage(currentChat, "Вы успешно записаны", replyMarkup: Keyboards.firstStep, cancellationToken: _ct);

                            await _createAppointmentService.RefreshSteps();
                        }
                        else if (steps.Count == 3)
                        {
                            var slots = await _slotService.GetCurrentDayAvailableTimeSlots(currentStep.AppointmentDate, ct);

                            await botClient.SendMessage(currentChat, "Выберите время", replyMarkup: TimeSlotsKeyboard(slots), cancellationToken: _ct);
                        }
                        break;


                    case Command.Exit:
                        Console.WriteLine("ExitExit");
                        break;

                    default:
                        await botClient.SendMessage(
                            currentChat,
                            "Ошибка: введена некорректная команда. Пожалуйста, введите команду заново.\n",
                            replyMarkup: currentUser != null ? Keyboards.start : Keyboards.firstStep,
                            cancellationToken: _ct);

                        await _createAppointmentService.RefreshSteps();

                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CallbackQuery callbackQuery, CancellationToken ct)
        {
            Chat currentChat = update.CallbackQuery.Message.Chat;
            long telegramCurrentUserId = update.CallbackQuery.From.Id;
            string telegramCurrentUserName = update.CallbackQuery.From.Username;

            string input = update.CallbackQuery.Data;

            string eventMessage = "";

            try
            {
                var currentUser = await _userService.GetUser(telegramCurrentUserId, _ct);

                var currentUserTaskList = currentUser != null
                    ? await _appointmentService.GetUserAppointmentsByUserId(currentUser.UserId, _ct)
                    : null;

                if (currentUser == null)
                {
                    if (input != "/start" && input != "Старт")
                    {
                        await botClient.SendMessage(currentChat, "Для запуска бота необходимо нажать на кнопку ниже или ввести /start", replyMarkup: Keyboards.start, cancellationToken: _ct);
                        return;
                    }
                }

                //присваиваю начальное значение введёного сообщения
                eventMessage = input;

                //НАЧАЛО ОБРАБОТКИ СООБЩЕНИЯ
                OnHandleUpdateStarted?.Invoke(eventMessage);

                (string inputCommand, string inputText, string chooseMonth, DateOnly appointmentDate, TimeOnly appointmentTime, Guid taskGuid) = Helper.InputCheck(input, currentUserTaskList);

                input = inputCommand.Replace("/", string.Empty);


                //объявление типа данных команды
                Command command;

                if (Enum.TryParse<Command>(input, true, out var result))
                    command = result;
                else
                    command = default;


                //объявление типа данных типа маникюра
                ManicureType manicureType;

                if (Enum.TryParse<ManicureType>(input, true, out var type))
                    manicureType = type;
                else
                    manicureType = default;

                Console.WriteLine(input);

                //КОНЕЦ ОБРАБОТКИ СООБЩЕНИЯ
                OnHandleUpdateCompleted?.Invoke(eventMessage);


                //switch (manicureType)
                //{
                //    case ManicureType.None:
                //        break;

                //    default:
                //        ProcedureFactory.CreateProcedure(input, out IProcedure procedure);

                //        await _createAppointmentService.AddStep(procedure);

                //        var calendarMarkup = CalendarGenerator.GenerateCalendar(DateTime.Today);

                //        await botClient.SendMessage(currentChat, "Выберите дату", replyMarkup: calendarMarkup, cancellationToken: _ct);

                //        return;
                //}

                //проверка списка с шагами записи
                var steps = await _createAppointmentService.GetSteps();

                CreateAppointmentTemplate currentStep = await _createAppointmentService.GetStep();

                if (steps.Count < 0)
                    throw new ApplicationException("Недопустимое значение количество шагов");

                //обработка основных команд
                switch (command)
                {

                    //case Command.AddAppointment:
                    //    ProcedureFactory.CreateProcedure(inputText, out IProcedure procedure);
                    //    await _appointmentService.AddAppointment(currentUser, procedure, DateTime.Now, _ct);
                    //    break;

                    case Command.Del:
                        await _appointmentService.CancelAppointment(taskGuid, _ct);
                        break;

                    case Command.Add:
                        //ProcedureFactory.CreateProcedure(inputText, out IProcedure procedure);
                        //await _appointmentService.AddAppointment(currentUser, procedure, DateTime.Now, _ct);
                        break;

                    case Command.CancelAppointment:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("CancelAppointment");
                        break;

                    case Command.EditAppointment:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("EditAppointmentEditAppointment");
                        break;

                    case Command.UpdateAppointment:
                        await _appointmentService.UpdateAppointment(taskGuid, AppointmentState.Completed, _ct);
                        break;

                    //команды создания записи
                    case Command.Create:
                        await botClient.SendMessage(currentChat, "Куда записываемся?", replyMarkup: Keyboards.secondStep, cancellationToken: _ct);
                        break;

                    case Command.Date:
                        await _createAppointmentService.AddStep(currentStep.Procedure, appointmentDate);
                        await botClient.SendMessage(currentChat, $"Выбранная дата - {appointmentDate}\n\nВерно?", replyMarkup: Keyboards.approveDate, cancellationToken: _ct);

                        break;

                    case Command.Time:
                        await _createAppointmentService.AddStep(currentStep.Procedure, currentStep.AppointmentDate, appointmentTime);
                        await botClient.SendMessage(currentChat, $"Выбранное время - {appointmentTime}\n\nВерно?", replyMarkup: Keyboards.approveTime, cancellationToken: _ct);

                        break;

                    case Command.Changemonth:
                        if (DateTime.TryParseExact(chooseMonth, "MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDisplayMonth))
                        {
                            var newCalendarMarkup = DaySlotsKeyboard(
                                newDisplayMonth,
                                DateTime.Today,
                                DateTime.Today.AddDays(60),
                                await _slotService.GetUnavailableDaySlots(ct)
                                );

                            await botClient.EditMessageReplyMarkup(currentChat, update.CallbackQuery.Message.Id, replyMarkup: newCalendarMarkup, cancellationToken: _ct);
                        }
                        break;


                    case Command.Exit:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("ExitExit");
                        break;

                    default:
                        await botClient.SendMessage(
                            currentChat, 
                            "Ошибка: введена некорректная команда. Пожалуйста, введите команду заново.\n", 
                            replyMarkup: currentUser != null ? Keyboards.start : Keyboards.firstStep, 
                            cancellationToken: _ct);

                        await _createAppointmentService.RefreshSteps();

                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        private ReplyKeyboardMarkup TimeSlotsKeyboard(Dictionary<TimeOnly, bool> slots)
        {
            if (slots.Count < 0)
            {
                Console.WriteLine("На выбранную дату записей нет");
            }
            var buttons = slots
                .Select(button => new KeyboardButton(button.Key.ToString()))
                .Select(btn => new[] { btn })
                .ToArray();

            return new ReplyKeyboardMarkup(buttons)
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

        #region МЕТОДЫ КОМАНД
        private async Task ShowAppointments(
            Guid userId, 
            ITelegramBotClient botClient, 
            Chat currentChat, 
            CancellationToken ct, 
            bool isActive = false, 
            IReadOnlyList<Appointment>? appointments = null)
        {
            //присвою список через оператор null объединения appointment
            var appointmentsList = appointments ?? (isActive
                ? await _appointmentService.GetUserAppointmentsByUserId(userId, ct)
                : await _appointmentService.GetUserActiveAppointmentsByUserId(userId, ct));

            if (appointmentsList.Count == 0)
            {
                string emptyMessage = isActive ? "Список записей пуст.\n" : "Aктивных записей нет";
                await botClient.SendMessage(currentChat, emptyMessage, cancellationToken: ct);
                return;
            }

            //выберу текст меседжа через тернарный оператор
            string message = appointments != null ? "Список найденных записей:"
                : (isActive ? "Список всех записей:" : "Список активных записей:");

            await botClient.SendMessage(currentChat, message, cancellationToken: ct);

            await Helper.AppointmentsListRender(appointmentsList, botClient, currentChat, ct);
        }

        private async Task ShowHelp(ITelegramBotClient botClient, Chat currentChat, BeautyBotUser user, CancellationToken ct)
        {
            if (user == null)
            {
                await botClient.SendMessage(currentChat, $"Незнакомец, это BeautyBot - телеграм бот записи на бьюти процедуры.\n" +
                $"Введя команду \"/start\" бот предложит тебе ввести имя\n" +
                $"Введя команду \"/help\" ты получишь справку о командах бота\n" +
                $"Введя команду \"/info\" ты получишь информацию о версии бота\n" +
                $"Введя команду \"/exit\" бот попрощается и завершит работу\n", cancellationToken: ct);
            }
            else
            {
                await botClient.SendMessage(currentChat, $"{user.TelegramUserName}, это BeautyBot - телеграм бот записи на бьюти процедуры.\n" +
                $"Введя команду \"/start\" команда регистрации в боте\n" +
                $"Введя команду \"/help\" ты получишь справку о командах ,jnf\n" +
                $"Введя команду \"/add\" *название задачи*\" ты сможешь записаться на процедуру\n" +
                $"Введя команду \"/showprocedures\" ты сможешь увидеть список активных записей в списке\n" +
                $"Введя команду \"/showallprocedures\" ты сможешь увидеть список всех записей в списке\n" +
                $"Введя команду \"/cancelprocedure \" *номер задачи*\" ты сможешь отменить запись\n" +
                $"Введя команду \"/find\" *название задачи*\" ты сможешь увидеть список всех задач начинающихся с названия задачи\n" +
                $"Введя команду \"/report\" ты получишь отчёт по записям\n" +
                $"Введя команду \"/info\" ты получишь информацию о версии программы\n" +
                $"Введя команду \"/exit\" бот попрощается и завершит работу\n", cancellationToken: ct);
            }
        }

        private async Task ShowInfo(ITelegramBotClient botClient, Chat currentChat, CancellationToken ct)
        {
            DateTime releaseDate = new DateTime(2025, 06, 23);
            await botClient.SendMessage(currentChat, $"Это BeautyBot версии 1.0 Beta. Релиз {releaseDate}.\n", cancellationToken: ct);
        }


        private async Task HandleBackCommand(
            Update update,
            ITelegramBotClient botClient, 
            long chatId, 
            List<CreateAppointmentTemplate> steps, 
            CreateAppointmentTemplate currentStep, 
            CancellationToken ct,
            string message = "Выберите дату")
        {
            var step = steps.Count;

            if (step > 0)
                steps.RemoveAt(step - 1); // Удаляем последний шаг

            if (currentStep != null)
            {
                var currentStepNumber = currentStep.Procedure switch
                {
                    Pedicure => Constants.StepsConfigPedicure.ElementAt(step),
                    Manicure => Constants.StepsConfigManicure.ElementAt(step),
                    _ => throw new NotSupportedException($"Unsupported procedure type: {currentStep.Procedure.GetType()}")
                };


                if (currentStepNumber.Message == "Выберите дату" || currentStepNumber.Message == "Выберите другую дату")
                {
                    var calendarMarkup = DaySlotsKeyboard(
                        DateTime.Today,
                        DateTime.Today,
                        DateTime.Today.AddDays(60),
                        await _slotService.GetUnavailableDaySlots(ct)
                        );

                    await botClient.SendMessage(chatId, "Выберите другую дату", replyMarkup: calendarMarkup, cancellationToken: _ct);


                    return;
                } 
                else if (currentStepNumber.Message == "Выберите время" || currentStepNumber.Message == "Выберите другое время")
                {
                    var slots = await _slotService.GetCurrentDayAvailableTimeSlots(currentStep.AppointmentDate, ct);

                    await botClient.SendMessage(chatId, "Выберите другое время", replyMarkup: TimeSlotsKeyboard(slots), cancellationToken: _ct);
                }
                else if (currentStepNumber.Message == "Выберите маникюр" || currentStepNumber.Message == "Выберите педикюр")
                {
                    await botClient.SendMessage(
                        chatId: chatId,
                        //тут надо починить в зависимости от маниюкра или педикюра
                        text: Constants.StepsConfigManicure.ElementAt(2).Message,
                        replyMarkup: currentStepNumber.Message == "Выберите маникюр" ? Keyboards.thirdManicureStep : Keyboards.thirdPedicureStep);
                }
                else if (currentStepNumber.Message == "Куда записываемся?")
                {
                    await botClient.SendMessage(
                        chatId: chatId,
                        text: Constants.StepsConfigManicure.ElementAt(1).Message,
                        replyMarkup: Keyboards.secondStep);
                }
            }
            else
            {
                // Возврат к началу, если что-то пошло не так
                await botClient.SendMessage(
                    chatId: chatId,
                    text: Constants.StepsConfigManicure.ElementAt(0).Message,
                    replyMarkup: Keyboards.firstStep);

                steps.Clear();
            }
        }

        #endregion


        public Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            Console.WriteLine($"Обработанное исключение: {exception.Message}");

            return Task.CompletedTask;
        }

        public void HandleStart(string message)
        {
            Console.WriteLine($"Началась обработка сообщения \"{message}\"\n");
        }

        public void HandleComplete(string message)
        {
            Console.WriteLine($"Закончилась обработка сообщения \"{message}\"\n");
        }
    }
}

