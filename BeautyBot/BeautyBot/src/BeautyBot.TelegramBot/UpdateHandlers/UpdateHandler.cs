using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Core.Enums;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Globalization;
using BeautyBot.src.BeautyBot.TelegramBot.Scenario;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using Telegram.Bot.Types.ReplyMarkups;
using System;

namespace BeautyBot.src.BeautyBot.TelegramBot.UpdateHandlers
{
    public delegate void MessageEventHandler(string message);
    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly ISlotService _slotService;

        //новый сервис сообщений
        private readonly IMessageService _messageService;

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
                await OnMessage(update, ct);
            }
            else if (update.CallbackQuery != null)
            {
                await OnCallbackQuery(update, ct);
            }
            else
            {
                //await OnUnknown();
                return;
            }
        }

        private async Task OnMessage(Update update, CancellationToken ct)
        {
            var messageData = await Helper.HandleMessageAsyncGetData(update, _userService, ct);

            string eventMessage = messageData.UserInput;

            try
            {
                if (messageData.User == null && (messageData.UserInput != "/start" && messageData.UserInput != "Старт"))
                {
                    await _messageService.SendMessage(messageData.Chat, "Для запуска бота необходимо нажать на кнопку ниже или ввести /start", Keyboards.start, ct);
                    return;
                }

                OnHandleUpdateStarted?.Invoke(eventMessage);

                var scenarioContext = await _scenarioContextRepository.GetContext(messageData.TelegramUserId, ct);

                (bool isHandled, ScenarioContext updatedContext) = await TryHandleOnMessageCommandAsync(update.Message, messageData, scenarioContext, ct);

                OnHandleUpdateCompleted?.Invoke(eventMessage);

                if (!isHandled)
                    return;

                if (scenarioContext != null)
                {
                    await ProcessScenario(scenarioContext, messageData, ct);

                    return;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnCallbackQuery(Update update, CancellationToken ct)
        {
            var messageData = await Helper.HandleMessageAsyncGetData(update, _userService, ct);

            string eventMessage = messageData.UserInput;

            try
            {
                if (messageData.User == null && (messageData.UserInput != "/start" && messageData.UserInput != "Старт"))
                {
                    await _messageService.SendMessage(messageData.Chat, "Для запуска бота необходимо нажать на кнопку ниже или ввести /start", Keyboards.start, ct);
                    return;
                }

                OnHandleUpdateStarted?.Invoke(eventMessage);

                var scenarioContext = await _scenarioContextRepository.GetContext(messageData.TelegramUserId, ct);

                var isHandled = await TryHandleCallbackCommandAsync(messageData, scenarioContext, ct);

                OnHandleUpdateCompleted?.Invoke(eventMessage);

                if (!isHandled)
                    return;                

                if (scenarioContext != null)
                {
                    await ProcessScenario(scenarioContext, messageData, ct);
                    return;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region МЕТОДЫ КОМАНД
        private async Task HandleStartCommand(Message updateMessage, BeautyBotUser user, CancellationToken ct)
        {
            if (user == null)
                user = await _userService.RegisterUser(
                    updateMessage.From.Id, 
                    updateMessage.From.Username, 
                    updateMessage.From.FirstName, 
                    updateMessage.From.LastName, 
                    _ct);

            await _messageService.SendMessage(updateMessage.Chat, $"Спасибо за регистрацию, {(string.IsNullOrEmpty(user.TelegramUserFirstName) ? user.TelegramUserName : user.TelegramUserFirstName)} 🤗", Keyboards.firstStep, ct);
        }

        private async Task HandleChangeDateCommand(Chat chat, ScenarioContext context, int messageId, CancellationToken ct)
        {
            await _messageService.DeleteMultiMessage(chat, [messageId - 2, messageId - 1], ct);

            var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

            context.DataHistory.Pop();

            context.CurrentStep = "DateProcedure";

            context.Data[context.CurrentStep] = null;

            var messagesToSend = new List<(string messages, ReplyMarkup keyboard)>
            {
                ("Выберите другую дату", Keyboards.cancelOrBack),
                ("✖ - означает, что на выбранную дату нет свободных слотов", Keyboards.DaySlotsKeyboard(DateTime.Today, unavailableSlots)),
            };

            await _messageService.SendMultiMessage(chat, messagesToSend, ct);
        }

        private async Task HandleChangeTimeCommand(ScenarioContext context, CancellationToken ct)
        {
            context.DataHistory.Pop();

            context.CurrentStep = "ApproveDateProcedure";
        }

        private async Task<(bool isHandled, ScenarioContext updatedContext)> HandleBackCommand(Chat chat, ScenarioContext context, long telegramUserId, CancellationToken ct)
        {
            if (context.CurrentStep == "BaseProcedure")
            {
                await _scenarioContextRepository.ResetContext(telegramUserId, ct);

                await _messageService.SendMessage(chat, "Вы в главном меню. Что хотите сделать?\n", Keyboards.firstStep, ct);

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

            if (context.CurrentStep == "DateProcedure")
                context.CurrentStep = "TypeProcedure";

            if (context.DataHistory?.Count > 0)
                context.DataHistory.Pop();

            return (true, context);
        }

        private async Task<(bool isHandled, ScenarioContext updatedContext)> HandleCancelCommand(Chat chat, long telegramUserId, CancellationToken ct)
        {
            await _scenarioContextRepository.ResetContext(telegramUserId, ct);

            await _messageService.SendMessage(chat, "Создание записи отменено. Вы в главном меню. Что хотите сделать?\n", Keyboards.firstStep, ct);

            return (false, null);
        }
        
        private async Task<(bool isHandled, ScenarioContext updatedContext)> HandleCreateCommand(ScenarioContext context, MessageData messageData, CancellationToken ct)
        {
            await ProcessScenario(
                Helper.CreateScenarioContext(ScenarioType.AddAppointment, messageData.TelegramUserId),
                messageData,
                ct);

            return (true, context);
        }

        private async Task HandleShowAppointmentsCommand(
            Guid userId, 
            Chat chat, 
            CancellationToken ct, 
            bool isActive = false, 
            IReadOnlyList<Appointment>? appointments = null)
        {

            //var currentUserTaskList = currentUser != null
            //? await _appointmentService.GetUserAppointmentsByUserId(currentUser.UserId, _ct)
            //: null;
            var appointmentsList = appointments ?? (isActive
                ? await _appointmentService.GetUserAppointmentsByUserId(userId, ct)
                : await _appointmentService.GetUserActiveAppointmentsByUserId(userId, ct));

            if (appointmentsList.Count == 0)
            {
                string emptyMessage = isActive ? "Список записей пуст.\n" : "Aктивных записей нет";
                await _messageService.SendMessage(chat, emptyMessage, Keyboards.firstStep, ct);
                return;
            }

            string message = appointments != null ? "Список найденных записей:"
                : (isActive ? "Список всех записей:" : "Список активных записей:");

            await _messageService.SendMessage(chat, message, Keyboards.firstStep, ct);

            //await Helper.AppointmentsListRender(appointmentsList, botClient, currentChat, ct);
        }

        private async Task HandleHelpCommand(Chat chat, BeautyBotUser user, CancellationToken ct)
        {
            if (user == null)
            {
                await _messageService.SendMessage(chat, $"Незнакомец, это BeautyBot - телеграм бот записи на бьюти процедуры.\n" +
                $"Введя команду \"/start\" бот предложит тебе ввести имя\n" +
                $"Введя команду \"/help\" ты получишь справку о командах бота\n" +
                $"Введя команду \"/info\" ты получишь информацию о версии бота\n" +
                $"Введя команду \"/exit\" бот попрощается и завершит работу\n", Keyboards.firstStep, ct);
            }
            else
            {
                await _messageService.SendMessage(chat, $"{user.TelegramUserName}, это BeautyBot - телеграм бот записи на бьюти процедуры.\n" +
                $"Введя команду \"/start\" команда регистрации в боте\n" +
                $"Введя команду \"/help\" ты получишь справку о командах ,jnf\n" +
                $"Введя команду \"/add\" *название задачи*\" ты сможешь записаться на процедуру\n" +
                $"Введя команду \"/showprocedures\" ты сможешь увидеть список активных записей в списке\n" +
                $"Введя команду \"/showallprocedures\" ты сможешь увидеть список всех записей в списке\n" +
                $"Введя команду \"/cancelprocedure \" *номер задачи*\" ты сможешь отменить запись\n" +
                $"Введя команду \"/find\" *название задачи*\" ты сможешь увидеть список всех задач начинающихся с названия задачи\n" +
                $"Введя команду \"/report\" ты получишь отчёт по записям\n" +
                $"Введя команду \"/info\" ты получишь информацию о версии программы\n" +
                $"Введя команду \"/exit\" бот попрощается и завершит работу\n", Keyboards.firstStep, ct);
            }
        }

        private async Task HandleInfoCommand(Chat chat, CancellationToken ct)
        {
            DateTime releaseDate = new DateTime(2025, 06, 23);
            await _messageService.SendMessage(chat, $"Это BeautyBot версии 1.0 Beta. Релиз {releaseDate}.\n", Keyboards.firstStep, ct);
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

        private async Task ProcessScenario(ScenarioContext context, MessageData messageData, CancellationToken ct)
        {
            var scenario = GetScenario(context.CurrentScenario);

            var scenarioResponse = await scenario.HandleMessageAsync(context, messageData, ct);

            context.LastResponse = scenarioResponse;

            //отправка сообщений
            if (scenarioResponse.Messages != null && scenarioResponse.Messages.Count > 1)
                await _messageService.SendMultiMessage(scenarioResponse.Chat, scenarioResponse.Messages, ct);
            else
                await _messageService.SendMessage(scenarioResponse.Chat, scenarioResponse.Message, scenarioResponse.Keyboard, ct);

            //установка-сброс контекста
            if (context.LastResponse.Result == ScenarioResult.Completed)
                await _scenarioContextRepository.ResetContext(messageData.TelegramUserId, ct);
            else
                await _scenarioContextRepository.SetContext(messageData.TelegramUserId, context, ct);
        }
        #endregion

        #region МЕТОДЫ ОБРАБОТЧИКИ КОМАНД

        private async Task<(bool isHandled, ScenarioContext updatedContext)> TryHandleOnMessageCommandAsync(Message updateMessage, MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            (string inputCommand, string chooseMonth, DateOnly chooseDate) = Helper.InputCheck(messageData.UserInput);

            inputCommand = inputCommand.Replace("/", string.Empty); 

            //парсинг команды
            Command command = Helper.GetEnumValueOrDefault<Command>(inputCommand); 

            //обработка основных команд 
            switch (command)
            {
                case Command.Start:
                    await HandleStartCommand(updateMessage, messageData.User, ct);
                    break;

                case Command.Help:
                    await HandleHelpCommand(messageData.Chat, messageData.User, _ct);
                    break;

                case Command.Info:
                    await HandleInfoCommand(messageData.Chat, _ct);
                    break;

                case Command.Show:
                    await HandleShowAppointmentsCommand(messageData.User.UserId, messageData.Chat, _ct, true);
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
                    await HandleChangeDateCommand(messageData.Chat, context, messageData.MessageId, ct);
                    break;

                case Command.ChangeTime:
                    await HandleChangeTimeCommand(context, ct);
                    break;


                case Command.FindAppointment:
                    //await ShowHelp(currentUser);
                    Console.WriteLine("FindAppointmentFindAppointment");
                    break;

                case Command.Back:
                    return await HandleBackCommand(messageData.Chat, context, messageData.TelegramUserId, ct);

                case Command.Cancel:
                    return await HandleCancelCommand(messageData.Chat, messageData.TelegramUserId, ct);

                case Command.Create:
                    return await HandleCreateCommand(context, messageData, ct);


                case Command.Exit:
                    Console.WriteLine("ExitExit");
                    break;

                default:
                    await _messageService.SendMessage(
                        messageData.Chat, 
                        "Ошибка: введена некорректная команда. Пожалуйста, введите команду заново.\n",
                        messageData.User != null ? Keyboards.firstStep : Keyboards.start,
                        ct);

                    return (false, context);
            }
            return (true, context);
        }


        private async Task<bool> TryHandleCallbackCommandAsync(MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            (string inputCommand, string chooseMonth, DateOnly chooseDate) = Helper.InputCheck(messageData.UserInput);

            inputCommand = inputCommand.Replace("/", string.Empty);

            //парсинг команды
            Command command = Helper.GetEnumValueOrDefault<Command>(inputCommand);

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

                        //await botClient.EditMessageReplyMarkup(chat, messageId, replyMarkup: Keyboards.DaySlotsKeyboard(newDisplayMonth, unavailableSlots), cancellationToken: _ct);

                        await _messageService.EditMessage(messageData.Chat, messageData.MessageId, Keyboards.DaySlotsKeyboard(newDisplayMonth, unavailableSlots), ct);
                    }
                    return false;

                case Command.Exit:
                    //await ShowHelp(currentUser);
                    Console.WriteLine("ExitExit");
                    break;

                default:
                    await _messageService.SendMessage(
                        messageData.Chat,
                        "Ошибка: введена некорректная команда. Пожалуйста, введите команду заново.\n",
                        messageData.User != null ? Keyboards.firstStep : Keyboards.start,
                        ct);

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

