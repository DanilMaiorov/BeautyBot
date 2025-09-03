using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Core.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;
using BeautyBot.src.BeautyBot.TelegramBot.Scenario;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using System.Collections.Generic;

namespace BeautyBot.src.BeautyBot.TelegramBot.UpdateHandlers
{
    public delegate void MessageEventHandler(string message);
    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly ISlotService _slotService;

        //логика сценариев
        private readonly IEnumerable<IScenario> _scenarios;
        private readonly IScenarioContextRepository _scenarioContextRepository;

        private readonly CancellationToken _ct;

        //добавлю 2 события
        public event MessageEventHandler OnHandleUpdateStarted;
        public event MessageEventHandler OnHandleUpdateCompleted;

        public UpdateHandler(
            IUserService userService,
            IAppointmentService appointmentService,
            PostgreSqlProcedureRepository procedureRepository,

            ISlotService slotService,

            IEnumerable<IScenario> scenarios,
            IScenarioContextRepository contextRepository,

            CancellationToken ct)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));

            _slotService = slotService;

            _scenarios = scenarios;
            _scenarioContextRepository = contextRepository;

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
            string telegramUserFirstName = update.Message.From.FirstName;
            string telegramUserLastName = update.Message.From.LastName;
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

                (string inputCommand, string chooseMonth, DateOnly chooseDate) = Helper.InputCheck(input, currentUserTaskList);

                input = inputCommand.Replace("/", string.Empty);

                //парсинг команды
                Command command = Helper.GetEnumValueOrDefault<Command>(input);

                //Работа с командой cancel и сценариями
                if (command == Command.Cancel)
                    await _scenarioContextRepository.ResetContext(telegramCurrentUserId, ct);

                var scenarioContext = await _scenarioContextRepository.GetContext(telegramCurrentUserId, ct);

                if (command == Command.Back)
                {
                    if (scenarioContext.CurrentStep == "BaseProcedure")
                    {
                        await _scenarioContextRepository.ResetContext(telegramCurrentUserId, ct);
                        scenarioContext = null;
                        command = Command.Main;
                    }
                    else
                    {
                        var lastKey = scenarioContext.Data.Keys.Last();

                        scenarioContext.CurrentStep = lastKey;

                        scenarioContext.Data.Remove(lastKey);
                    }
                }

                if (scenarioContext != null)
                {
                    await ProcessScenario(botClient, telegramCurrentUserId, scenarioContext, update, ct);

                    return;
                }

                Console.WriteLine(input);

                //КОНЕЦ ОБРАБОТКИ СООБЩЕНИЯ
                OnHandleUpdateCompleted?.Invoke(eventMessage);

                //обработка основных команд
                switch (command)
                {
                    case Command.Start:
                        if (currentUser == null)
                            currentUser = await _userService.RegisterUser(telegramCurrentUserId, telegramCurrentUserName, telegramUserFirstName, telegramUserLastName, _ct);

                        await botClient.SendMessage(
                            currentChat, 
                            $"Спасибо за регистрацию, {(string.IsNullOrEmpty(telegramUserFirstName) ? telegramCurrentUserName : telegramUserFirstName)} 🤗", 
                            replyMarkup: Keyboards.firstStep, 
                            cancellationToken: _ct);
                        //await Helper.CommandsRender(currentChat, botClient, _ct);
                        break;

                    case Command.Main:
                        await botClient.SendMessage(
                        currentChat,
                        "Что хотите сделать?\n",
                        replyMarkup: Keyboards.firstStep,
                        cancellationToken: _ct);

                        break;

                    case Command.Help:
                        await ShowHelp(botClient, currentChat, currentUser, _ct);
                        break;

                    case Command.Info:
                        await ShowInfo(botClient, currentChat, _ct);
                        break;

                     case Command.Show:
                        await ShowAppointments(currentUser.UserId, botClient, currentChat, _ct, true);
                        break;

                    case Command.Del:
                        //await _appointmentService.CancelAppointment(taskGuid, _ct);
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
                        //await _appointmentService.UpdateAppointment(taskGuid, AppointmentState.Completed, _ct);
                        break;

                    case Command.FindAppointment:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("FindAppointmentFindAppointment");
                        break;

                    //команды создания записи
                    case Command.Create:
                        await ProcessScenario(
                            botClient,
                            telegramCurrentUserId,
                            Helper.CreateScenarioContext(ScenarioType.AddAppointment, telegramCurrentUserId),
                            update,
                            ct);

                        break;


                    case Command.Exit:
                        Console.WriteLine("ExitExit");
                        break;

                    default:
                        await botClient.SendMessage(
                            currentChat,
                            "Ошибка: введена некорректная команда. Пожалуйста, введите команду заново.\n",
                            replyMarkup: currentUser != null ? Keyboards.firstStep : Keyboards.start,
                            cancellationToken: _ct);

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

                var currentUserAppointmentsList = currentUser != null
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

                (string inputCommand, string chooseMonth, DateOnly chooseDate) = Helper.InputCheck(input, currentUserAppointmentsList);

                input = inputCommand.Replace("/", string.Empty);

                //парсинг команды
                Command command = Helper.GetEnumValueOrDefault<Command>(input);

                var scenarioContext = await _scenarioContextRepository.GetContext(telegramCurrentUserId, ct);

                //Работа со стрелками смены месяца
                if (command == Command.Changemonth)
                {
                    if (DateTime.TryParseExact(chooseMonth, "MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDisplayMonth) && scenarioContext != null)
                    {
                        var newCalendarMarkup = DaySlotsKeyboard(
                            newDisplayMonth,
                            DateTime.Today,
                            DateTime.Today.AddDays(60),
                            await _slotService.GetUnavailableSlotsByDate(ct)
                            );

                        await botClient.EditMessageReplyMarkup(currentChat, update.CallbackQuery.Message.Id, replyMarkup: newCalendarMarkup, cancellationToken: _ct);
                    }
                    return; 
                }    

                if(command == Command.Date && (scenarioContext == null || scenarioContext.CurrentStep != "DateProcedure"))
                     return;

                if (scenarioContext != null)
                {
                    await ProcessScenario(botClient, telegramCurrentUserId, scenarioContext, update, ct);

                    return;
                }

                //КОНЕЦ ОБРАБОТКИ СООБЩЕНИЯ
                OnHandleUpdateCompleted?.Invoke(eventMessage);

                //обработка основных команд
                switch (command)
                {
                    case Command.CancelAppointment:
                        Console.WriteLine("CancelAppointment");
                        break;

                    case Command.EditAppointment:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("EditAppointmentEditAppointment");
                        break;

                    case Command.Exit:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("ExitExit");
                        break;

                    default:
                        await botClient.SendMessage(
                            currentChat, 
                            "Введена некорректная команда. Пожалуйста, введите команду заново.\n", 
                            replyMarkup: currentUser != null ? Keyboards.firstStep : Keyboards.start, 
                            cancellationToken: _ct);

                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
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

            var dayNamesRow = new List<InlineKeyboardButton>();
            for (int i = 0; i < 7; i++)
            {
                dayNamesRow.Add(InlineKeyboardButton.WithCallbackData(
                    CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(i + (int)DayOfWeek.Monday) % 7],
                    "day_name_no_action"
                ));
            }
            keyboardButtons.Add(dayNamesRow);

            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);

            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

            var currentRow = new List<InlineKeyboardButton>();

            for (int i = 0; i < offset; i++)
                currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));

            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDay = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var currentDateOnly = DateOnly.FromDateTime(currentDay);

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

            if (currentRow.Any())
            {
                while (currentRow.Count < 7)
                    currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));

                keyboardButtons.Add(currentRow);
            }

            var navigationRow = new List<InlineKeyboardButton>();

            if (displayMonth.Year > minDate.Year || (displayMonth.Year == minDate.Year && displayMonth.Month > minDate.Month))
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                    "<",
                    $"{PrevMonthCallback}{displayMonth.AddMonths(-1):yyyy-MM-dd}"
                ));
            }
            else
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button"));
            }

            navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                displayMonth.ToString("MMMM yyyy", CultureInfo.CurrentCulture),
                "month_display_no_action"
            ));

            if (displayMonth.AddMonths(1) <= maxDate.AddDays(1).Date) 
            {
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
                    navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button")); 
                }
            }
            else
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button"));
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
            var appointmentsList = appointments ?? (isActive
                ? await _appointmentService.GetUserAppointmentsByUserId(userId, ct)
                : await _appointmentService.GetUserActiveAppointmentsByUserId(userId, ct));

            if (appointmentsList.Count == 0)
            {
                string emptyMessage = isActive ? "Список записей пуст.\n" : "Aктивных записей нет";
                await botClient.SendMessage(currentChat, emptyMessage, cancellationToken: ct);
                return;
            }

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

        #endregion

        #region МЕТОДЫ СЦЕНАРИЯ
        /// <summary>
        /// Возвращает экземпляр сценария по указанному типу.
        /// </summary>
        /// <param name="scenario">Тип сценария из перечисления ScenarioType</param>
        /// <returns>Реализация интерфейса IScenario для запрошенного сценария</returns>
        /// <exception cref="NotSupportedException">Выбрасывается при передаче неподдерживаемого значения ScenarioType</exception>
        private IScenario GetScenario(ScenarioType scenario)
        {
            var currentScenario = _scenarios.FirstOrDefault(s => s.CanHandle(scenario));
            return currentScenario ?? throw new NotSupportedException($"Сценарий {scenario} не поддерживается");
        }

        private async Task ProcessScenario(ITelegramBotClient botClient, long telegramUserId, ScenarioContext context, Update update, CancellationToken ct)
        {
            var scenario = GetScenario(context.CurrentScenario);

            var scenarioResult = await scenario.HandleMessageAsync(botClient, context, update, ct);

            if (scenarioResult == ScenarioResult.Completed)
                await _scenarioContextRepository.ResetContext(telegramUserId, ct);
            else
                await _scenarioContextRepository.SetContext(telegramUserId, context, ct);
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

