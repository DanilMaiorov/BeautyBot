using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Core.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Globalization;
using BeautyBot.src.BeautyBot.TelegramBot.Scenario;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;

namespace BeautyBot.src.BeautyBot.TelegramBot.UpdateHandlers
{
    public delegate void MessageEventHandler(string message);
    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly ISlotService _slotService;

        //новый сервис сообщений
        private IMessageService _messageService;

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

            IMessageService messageService,



            IEnumerable<IScenario> scenarios,
            IScenarioContextRepository contextRepository,
            CancellationToken ct)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _slotService = slotService;



            _messageService = messageService;



            _scenarios = scenarios;
            _scenarioContextRepository = contextRepository;
            _ct = ct;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (update.Message != null)
            {
                await OnMessage(botClient, update, ct);
            }
            else if (update.CallbackQuery != null)
            {
                await OnCallbackQuery(botClient, update, ct);
            }
            else
            {
                //await OnUnknown();
                return;
            }
        }

        private async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var messageData = await Helper.HandleMessageAsyncGetData(update, _userService, ct);

            string eventMessage = messageData.UserInput;

            await _messageService.SendMessage(messageData.Chat, "Тестовое сообщение через сервис", Keyboards.start, _ct);

            try
            {
                if (messageData.User == null && (messageData.UserInput != "/start" && messageData.UserInput != "Старт"))
                {
                    await botClient.SendMessage(messageData.Chat, "Для запуска бота необходимо нажать на кнопку ниже или ввести /start", replyMarkup: Keyboards.start, cancellationToken: _ct);
                    return;
                }

                OnHandleUpdateStarted?.Invoke(eventMessage);

                var scenarioContext = await _scenarioContextRepository.GetContext(messageData.TelegramUserId, ct);

                (bool isHandled, ScenarioContext updatedContext) = await TryHandleOnMessageCommandAsync(botClient, messageData, scenarioContext, ct);

                OnHandleUpdateCompleted?.Invoke(eventMessage);

                if (!isHandled)
                    return;

                if (scenarioContext != null)
                {
                    await ProcessScenario(botClient, scenarioContext, messageData, ct);

                    return;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var messageData = await Helper.HandleMessageAsyncGetData(update, _userService, ct);

            string eventMessage = messageData.UserInput;

            try
            {
                if (messageData.User == null && (messageData.UserInput != "/start" && messageData.UserInput != "Старт"))
                {
                    await botClient.SendMessage(messageData.Chat, "Для запуска бота необходимо нажать на кнопку ниже или ввести /start", replyMarkup: Keyboards.start, cancellationToken: _ct);
                    return;
                }

                OnHandleUpdateStarted?.Invoke(eventMessage);

                var scenarioContext = await _scenarioContextRepository.GetContext(messageData.TelegramUserId, ct);

                var isHandled = await TryHandleCallbackCommandAsync(botClient, messageData.Chat, messageData.UserInput, scenarioContext, messageData.MessageId, ct);

                OnHandleUpdateCompleted?.Invoke(eventMessage);

                if (!isHandled)
                    return;                

                if (scenarioContext != null)
                {
                    await ProcessScenario(botClient, scenarioContext, messageData, ct);
                    return;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region МЕТОДЫ КОМАНД


        private async Task HandleStartCommand(ITelegramBotClient botClient, Chat chat, BeautyBotUser user)
        {
            if (user == null)
                user = await _userService.RegisterUser(user, _ct);

            await botClient.SendMessage(
                chat,
                $"Спасибо за регистрацию, {(string.IsNullOrEmpty(user.TelegramUserFirstName) ? user.TelegramUserName : user.TelegramUserFirstName)} 🤗",
                replyMarkup: Keyboards.firstStep,
                cancellationToken: _ct);
            //await Helper.CommandsRender(currentChat, botClient, _ct);
        }

        private async Task HandleChangeDateCommand(ITelegramBotClient botClient, Chat chat, ScenarioContext context, int messageId, CancellationToken ct)
        {
            var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

            await botClient.DeleteMessage(chatId: chat, messageId: messageId - 2, cancellationToken: ct);
            await botClient.DeleteMessage(chatId: chat, messageId: messageId - 1, cancellationToken: ct);

            context.DataHistory.Pop();

            context.CurrentStep = "DateProcedure";

            context.Data[context.CurrentStep] = null;

            await botClient.SendMessage(chat, "Выберите другую дату", replyMarkup: Keyboards.cancelOrBack, cancellationToken: _ct);
            await botClient.SendMessage(chat, "✖ - означает, что на выбранную дату нет свободных слотов", replyMarkup: Keyboards.DaySlotsKeyboard(DateTime.Today, unavailableSlots), cancellationToken: _ct);
        }

        private async Task HandleChangeTimeCommand(ITelegramBotClient botClient, ScenarioContext context, CancellationToken ct)
        {
            context.DataHistory.Pop();

            context.CurrentStep = "ApproveDateProcedure";

            context.Data[context.CurrentStep] = null;
        }

        private async Task<(bool isHandled, ScenarioContext updatedContext)> HandleBackCommand(ITelegramBotClient botClient, Chat chat, ScenarioContext context, long telegramUserId, CancellationToken ct)
        {
            if (context.CurrentStep == "BaseProcedure")
            {
                await _scenarioContextRepository.ResetContext(telegramUserId, ct);

                await botClient.SendMessage(chat, "Вы в главном меню. Что хотите сделать?\n", replyMarkup: Keyboards.firstStep, cancellationToken: _ct);

                return (false, null);
            }
            else
            {
                if (context.Data?.Keys?.Any() == true)
                {
                    var lastKey = context.Data.Keys.Last();
                    context.Data.Remove(lastKey);
                }
            }

            context.CurrentStep = context.Data?.Keys?.LastOrDefault() ?? null;

            if (context.CurrentStep == "User")
                context.CurrentStep = null;

            if (context.DataHistory?.Count > 0)
                context.DataHistory.Pop();

            return (true, context);
        }

        private async Task<(bool isHandled, ScenarioContext updatedContext)> HandleCancelCommand(ITelegramBotClient botClient, Chat chat, long telegramUserId, CancellationToken ct)
        {
            await _scenarioContextRepository.ResetContext(telegramUserId, ct);

            await botClient.SendMessage(chat, "Создание записи отменено. Вы в главном меню. Что хотите сделать?\n", replyMarkup: Keyboards.firstStep, cancellationToken: _ct);

            return (false, null);
        }
        
        private async Task<(bool isHandled, ScenarioContext updatedContext)> HandleCreateCommand(ITelegramBotClient botClient, ScenarioContext context, MessageData messageData, CancellationToken ct)
        {
            await ProcessScenario(
                botClient,
                Helper.CreateScenarioContext(ScenarioType.AddAppointment, messageData.TelegramUserId),
                messageData,
                ct);

            return (true, context);
        }

        private async Task HandleShowAppointmentsCommand(
            Guid userId, 
            ITelegramBotClient botClient, 
            Chat currentChat, 
            CancellationToken ct, 
            bool isActive = false, 
            IReadOnlyList<Appointment>? appointments = null)
        {

    //        var currentUserTaskList = currentUser != null
    //? await _appointmentService.GetUserAppointmentsByUserId(currentUser.UserId, _ct)
    //: null;
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

        private async Task HandleHelpCommand(ITelegramBotClient botClient, Chat currentChat, BeautyBotUser user, CancellationToken ct)
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

        private async Task HandleInfoCommand(ITelegramBotClient botClient, Chat currentChat, CancellationToken ct)
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

        private async Task ProcessScenario(ITelegramBotClient botClient, ScenarioContext context, MessageData messageData, CancellationToken ct)
        {
            var scenario = GetScenario(context.CurrentScenario);

            var scenarioResponse = await scenario.HandleMessageAsync(context, messageData, ct);

            context.LastResponse = scenarioResponse;


            //await botClient.SendMessage(
            //    context.LastResponse.Chat,
            //    context.LastResponse.Message, 
            //    replyMarkup: context.LastResponse.Keyboard, 
            //    cancellationToken: ct);

            //завести интерфейс тут
            // Обработка нескольких сообщений
            if (scenarioResponse.Messages != null && scenarioResponse.Messages.Count > 0)
            {
                for (int i = 0; i < scenarioResponse.Messages.Count; i++)
                {
                    var keyboard = scenarioResponse.Keyboards != null && i < scenarioResponse.Keyboards.Count
                        ? scenarioResponse.Keyboards[i]
                        : null;

                    await botClient.SendMessage(
                        scenarioResponse.Chat,
                        scenarioResponse.Messages[i],
                        replyMarkup: keyboard,
                        cancellationToken: ct);
                }
            }
            // Обработка одиночного сообщения (для обратной совместимости)
            else if (!string.IsNullOrEmpty(scenarioResponse.Message))
            {
                await botClient.SendMessage(
                    scenarioResponse.Chat,
                    scenarioResponse.Message,
                    replyMarkup: scenarioResponse.Keyboard,
                    cancellationToken: ct);
            }


            if (context.LastResponse.Result == ScenarioResult.Completed)
                await _scenarioContextRepository.ResetContext(messageData.TelegramUserId, ct);
            else
                await _scenarioContextRepository.SetContext(messageData.TelegramUserId, context, ct);
        }
        #endregion

        #region МЕТОДЫ ОБРАБОТЧИКИ КОМАНД

        private async Task<(bool isHandled, ScenarioContext updatedContext)> TryHandleOnMessageCommandAsync(ITelegramBotClient botClient, MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            (string inputCommand, string chooseMonth, DateOnly chooseDate) = Helper.InputCheck(messageData.UserInput);

            inputCommand = inputCommand.Replace("/", string.Empty); 

            //парсинг команды
            Command command = Helper.GetEnumValueOrDefault<Command>(inputCommand); 

            //обработка основных команд 
            switch (command)
            {
                case Command.Start:
                    await HandleStartCommand(botClient, messageData.Chat, messageData.User);
                    break;

                case Command.Help:
                    await HandleHelpCommand(botClient, messageData.Chat, messageData.User, _ct);
                    break;

                case Command.Info:
                    await HandleInfoCommand(botClient, messageData.Chat, _ct);
                    break;

                case Command.Show:
                    await HandleShowAppointmentsCommand(messageData.User.UserId, botClient, messageData.Chat, _ct, true);
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

                case Command.Manicure:
                case Command.Pedicure:
                case Command.Classic:
                case Command.GelPolish:
                case Command.French:
                case Command.Time:
                case Command.Approve:
                    return (true, context);


                case Command.ChangeDate:
                    await HandleChangeDateCommand(botClient, messageData.Chat, context, messageData.MessageId, ct);
                    break;

                case Command.ChangeTime:
                    await HandleChangeTimeCommand(botClient, context, ct);
                    break;


                case Command.FindAppointment:
                    //await ShowHelp(currentUser);
                    Console.WriteLine("FindAppointmentFindAppointment");
                    break;

                case Command.Back:
                    return await HandleBackCommand(botClient, messageData.Chat, context, messageData.TelegramUserId, ct);

                case Command.Cancel:
                    return await HandleCancelCommand(botClient, messageData.Chat, messageData.TelegramUserId, ct);

                case Command.Create:
                    return await HandleCreateCommand(botClient, context, messageData, ct);


                case Command.Exit:
                    Console.WriteLine("ExitExit");
                    break;

                default:
                    
                    
                    await botClient.SendMessage(
                        messageData.Chat,
                        "Ошибка: введена некорректная команда. Пожалуйста, введите команду заново.\n",
                        replyMarkup: messageData.User != null ? Keyboards.firstStep : Keyboards.start,
                        cancellationToken: _ct);

                    return (false, context);
            }
            return (true, context);
        }


        private async Task<bool> TryHandleCallbackCommandAsync(ITelegramBotClient botClient, Chat chat, string input, ScenarioContext context, int messageId, CancellationToken ct)
        {

            (string inputCommand, string chooseMonth, DateOnly chooseDate) = Helper.InputCheck(input);

            input = inputCommand.Replace("/", string.Empty);

            //парсинг команды
            Command command = Helper.GetEnumValueOrDefault<Command>(input);

            //обработка основных команд
            switch (command)
            {
                case Command.CancelAppointment:
                    Console.WriteLine("CancelAppointment");

                    //var currentUserAppointmentsList = currentUser != null
                    //? await _appointmentService.GetUserAppointmentsByUserId(currentUser.UserId, _ct)
                    //: null;
                    break;

                case Command.EditAppointment:
                    //await ShowHelp(currentUser);
                    Console.WriteLine("EditAppointmentEditAppointment");
                    break;

                case Command.Date:
                    return context != null && context.CurrentStep == "DateProcedure";

                case Command.ChangeMonth:
                    if (DateTime.TryParseExact(chooseMonth, "MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDisplayMonth) && context != null)
                    {
                        var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

                        await botClient.EditMessageReplyMarkup(chat, messageId, replyMarkup: Keyboards.DaySlotsKeyboard(newDisplayMonth, unavailableSlots), cancellationToken: _ct);
//
//
//
//
//

                    }
                    return false;

                case Command.Exit:
                    //await ShowHelp(currentUser);
                    Console.WriteLine("ExitExit");
                    break;

                default:
                    await botClient.SendMessage(
                        chat,
                        "Введена некорректная команда. Пожалуйста, введите команду заново.\n",
                        replyMarkup: chat != null ? Keyboards.firstStep : Keyboards.start,
                        cancellationToken: _ct);

                    return false;
            }
            return true;
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

