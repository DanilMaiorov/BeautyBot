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
using BeautyBot.src.BeautyBot.TelegramBot.Dtos;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using NailBot.Helpers;
using static System.Net.Mime.MediaTypeNames;

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


        //количество кнопок задач на 1 странице
        int _pageSize = 5;

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

                //получение сценария
                var scenarioContext = await _scenarioContextRepository.GetContext(messageData.TelegramUserId, ct);

                await TryHandleOnMessageCommandAsync(update.Message, messageData, scenarioContext, ct);

                OnHandleUpdateCompleted?.Invoke(eventMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task OnCallbackQuery(Update update, CancellationToken ct)
        {
            //получение данных сообщения
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

                //получение сценария
                var scenarioContext = await _scenarioContextRepository.GetContext(messageData.TelegramUserId, ct);

                //метод обработки сообщения 
                await TryHandleCallbackCommandAsync(messageData, scenarioContext, ct);

                OnHandleUpdateCompleted?.Invoke(eventMessage);           
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region МЕТОДЫ ОБРАБОТЧИКИ КОМАНД

        private async Task TryHandleOnMessageCommandAsync(Message updateMessage, MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            (string inputCommand, string chooseMonth, DateOnly chooseDate) = Helper.NormalizeInput(messageData.UserInput);

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
                    await HandleShowAppointmentsCommand(messageData.User.UserId, messageData.Chat, messageData.MessageId, _ct);
                    break;

                case Command.UpdateAppointment:
                    //await _appointmentService.UpdateAppointment(taskGuid, AppointmentState.Completed, _ct);
                    break;

                case Command.Manicure:
                case Command.Pedicure:
                    await HandleBaseProcedureCommand(messageData, context, ct);
                    break;

                case Command.Classic:
                case Command.GelPolish:
                case Command.French:
                    await HandleTypeProcedureCommand(messageData, context, ct);
                    break;

                case Command.Time:
                    await HandleChooseTimeCommand(messageData, context, ct);
                    break;

                case Command.Approve:
                    await ProcessScenario(context, messageData, ct);
                    break;

                case Command.ChangeDate:
                    await HandleChangeDateCommand(messageData, context, ct);
                    break;

                case Command.ChangeTime:
                    await HandleChangeTimeCommand(messageData, context, ct);
                    break;

                case Command.FindAppointment:
                    //await ShowHelp(currentUser);
                    Console.WriteLine("FindAppointmentFindAppointment");
                    break;

                case Command.Back:
                    await HandleBackCommand(messageData, context, messageData.TelegramUserId, ct);
                    break;

                case Command.Cancel:
                    //тут надо сделать
                    await HandleCancelCommand(messageData.Chat, messageData.TelegramUserId, ct);
                    break;

                case Command.Create:
                    await HandleCreateCommand(messageData, ct);
                    break;

                case Command.Exit:
                    Console.WriteLine("ExitExit");
                    break;

                default:
                    await _messageService.SendMessage(
                        messageData.Chat,
                        "Ошибка: введена некорректная команда. Пожалуйста, введите команду заново.\n",
                        messageData.User != null ? Keyboards.firstStep : Keyboards.start,
                        ct);
                    break;
            }
        }

        private async Task TryHandleCallbackCommandAsync(MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            (string inputCommand, string chooseMonth, DateOnly chooseDate) = Helper.NormalizeInput(messageData.UserInput);

            inputCommand = inputCommand.Replace("/", string.Empty);

            var callbackDto = CallbackDto.FromString(messageData.UserInput);

            //парсинг команды
            Command command = Helper.GetEnumValueOrDefault<Command>(inputCommand);

            //обработка основных команд
            switch (callbackDto.Action)
            {
                case "list_appointments":
                    await HandleShowAppointmentsList(messageData, ct);
                    break;

                case "show_ap":
                    await HandleShowAppointmentDetails(messageData, ct);
                    break;

                case "cancel_appointment":
                    await HandleCancelAppointmentCommand(messageData, ct);
                    break;

                case "approve_cancel":
                    await HandleApproveCancelAppointmentCommand(messageData, context, ct);
                    break;

                case "decline_cancel":
                    await HandleDeclineCancelAppointmentCommand(messageData, ct);
                    break;

                case "edit_appointment":
                    Console.WriteLine("тут будет обработка изменения");
                    break;

                case "empty_day":
                case "month_display_no_action":
                case "day_name_no_action":
                case "day_unavailable":
                case "no_action":
                    Console.WriteLine("Не активные кнопки календаря или пагинации");
                    break;

                case "day_selected":
                    await HandleDaySelectCommand(context, messageData, ct);
                    break;

                case "prev_month":
                case "next_month":
                    await HandleChangeMonthCommand(context, messageData, chooseMonth, ct);
                    break;

                case "exit":
                    //await ShowHelp(currentUser);
                    Console.WriteLine("ExitExit");
                    break;

                default:
                    await _messageService.SendMessage(
                        messageData.Chat,
                        "Ошибка: введена некорректная команда. Пожалуйста, введите команду заново.\n",
                        messageData.User != null ? Keyboards.firstStep : Keyboards.start,
                        ct);
                    break;
            }
        }
        private async Task<IReadOnlyList<KeyValuePair<string, string>>> GetKeyValuePairAppointmentsCollection(Guid userId, CancellationToken ct)
        {
            var appointments = await _appointmentService.GetUserActiveAppointmentsByUserId(userId, ct);

            return appointments
                .ToReadOnlyKeyValueList(
                    item => item.Id.ToString(),
                    item => $"{Helper.GetRussianSubtypeName(item.Procedure)} {item.AppointmentDate.ToString("dd.MM")} в {item.AppointmentDate.ToString("HH:mm")}");
        }

        #endregion

        #region МЕТОДЫ ON MESSAGE КОМАНД
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

        private async Task HandleChangeDateCommand(MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            await _messageService.DeleteMultiMessage(messageData.Chat, [messageData.MessageId - 2, messageData.MessageId - 1], ct);

            var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

            context.DataHistory.Pop();

            context.CurrentStep = "TypeProcedure";

            //var messagesToSend = new List<(string messages, ReplyMarkup keyboard)>
            //{
            //    ("Выберите другую дату", Keyboards.cancelOrBack),
            //    ("✖ - означает, что на выбранную дату нет свободных слотов", Keyboards.DaySlotsKeyboard(DateTime.Today, unavailableSlots)),
            //};

            //await _messageService.SendMultiMessage(chat, messagesToSend, ct);

            await ProcessScenario(context, messageData, ct);




            //if (context == null)
            //    return;

            //await _messageService.DeleteMultiMessage(chat, [messageId - 2, messageId - 1], ct);

            //var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

            //context.DataHistory.Pop();

            //context.CurrentStep = "TypeProcedure";




            //if (calendarDayCallbackDto.Date == default)
            //    return false;

            //context.Data["DateProcedure"] = calendarDayCallbackDto.Date;

            //await ProcessScenario(context, messageData, ct);

            //return true;
        }

        private async Task HandleBaseProcedureCommand(MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            if (context.DataHistory.Count > 0)
                messageData.UserInput = context.DataHistory.Pop();

            if (messageData.UserInput != Constants.Manicure && messageData.UserInput != Constants.Pedicure)
                throw new Exception("Что-то пошло не так");

            context.Data["BaseProcedure"] = messageData.UserInput;

            context.DataHistory.Push(messageData.UserInput);

            context.CurrentStep = "BaseProcedure";

            await ProcessScenario(context, messageData, ct);
        }

        private async Task HandleTypeProcedureCommand(MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            if (context.DataHistory.Count > 1)
                messageData.UserInput = context.DataHistory.Pop();

            context.Data.TryGetValue("BaseProcedure", out var procedureBaseType);

            if (procedureBaseType == null)
                throw new Exception("Что-то пошло не так");

            context.Data.TryGetValue("TypeProcedure", out var procedureSubtype);

            context.DataHistory.Push(messageData.UserInput);

            if (procedureSubtype is IProcedure subtype && subtype != null)
                procedureSubtype = Helper.GetRussianSubtypeName(subtype);
            else
                procedureSubtype = context.DataHistory.Peek();

            context.Data["TypeProcedure"] = ProcedureFactory.CreateProcedure((string)procedureSubtype, (string)procedureBaseType);

            await ProcessScenario(context, messageData, ct);
        }

        private async Task HandleChooseTimeCommand(MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            if (!TimeOnly.TryParse(messageData.UserInput, out var time))
                throw new InvalidCastException($"Ожидался TimeOnly, получен {time.GetType().Name ?? "null"}");

            await Task.Delay(1, ct);

            context.DataHistory.Push(time.ToString());

            context.Data["TimeProcedure"] = time;

            await ProcessScenario(context, messageData, ct);
        }

        private async Task HandleChangeTimeCommand(MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            context.DataHistory.Pop();

            context.CurrentStep = "ApproveDateProcedure";

            await ProcessScenario(context, messageData, ct);
        }

        private async Task HandleBackCommand(MessageData messageData, ScenarioContext context, long telegramUserId, CancellationToken ct)
        {
            if (context == null)
                return;

            if (context.CurrentStep == "BaseProcedure")
            {
                await _scenarioContextRepository.ResetContext(telegramUserId, ct);

                await _messageService.SendMessage(messageData.Chat, "Вы в главном меню. Что хотите сделать?\n", Keyboards.firstStep, ct);

                return;
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

            if (context.CurrentStep == "DateProcedure" && context.DataHistory?.Count == 3)
                context.CurrentStep = "TypeProcedure";

            if (context.CurrentStep == "TypeProcedure" && context.DataHistory?.Count == 2)
                context.CurrentStep = "BaseProcedure";

            if (context.DataHistory?.Count > 0)
                context.DataHistory.Pop();

            await ProcessScenario(context, messageData, ct);
        }

        private async Task HandleCancelCommand(Chat chat, long telegramUserId, CancellationToken ct)
        {
            await _scenarioContextRepository.ResetContext(telegramUserId, ct);

            await _messageService.SendMessage(chat, "Создание записи отменено. Вы в главном меню. Что хотите сделать?\n", Keyboards.firstStep, ct);

            return;
        }
        
        private async Task HandleCreateCommand(MessageData messageData, CancellationToken ct)
        {
            await ProcessScenario(
                Helper.CreateScenarioContext(ScenarioType.AddAppointment, messageData.TelegramUserId),
                messageData,
                ct);
        }

        private async Task HandleShowAppointmentsCommand(Guid userId, Chat chat, int messageId, CancellationToken ct)
        {
            var appointmentsList = await GetKeyValuePairAppointmentsCollection(userId, ct);

            if (appointmentsList.Count == 0)
            {
                string emptyMessage = "Вы ещё никуда не записаны";
                await _messageService.SendMessage(chat, emptyMessage, Keyboards.firstStep, ct);
                return;
            }

            var pagedListDto = new PagedListCallbackDto { Action = "list_appointments", Page = 0 };

            var pagedKeyboard = await BuildPagedAppointmentButtons(appointmentsList, pagedListDto);

            await _messageService.SendMessage(chat, "Список активных записей:", pagedKeyboard, ct);
        }


        private async Task<InlineKeyboardMarkup> BuildPagedAppointmentButtons(IReadOnlyList<KeyValuePair<string, string>> callbackData, PagedListCallbackDto listDto)
        {
            var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>();

            var totalPages = (int)Math.Ceiling(((double)callbackData.Count / _pageSize));

            var currentPageTasks = callbackData.GetBatchByNumber(_pageSize, listDto.Page);

            Keyboards.GetAppointmentListKeyboardWithPagination(currentPageTasks, keyboardRows, listDto, totalPages);

            return new InlineKeyboardMarkup(keyboardRows);
        }
        private async Task HandleShowAppointmentDetails(MessageData messageData, CancellationToken ct)
        {
            var appointmentDto = AppointmentCallbackDto.FromString(messageData.UserInput);

            if (appointmentDto.AppointmentId == null)
                throw new Exception("Беда с id записи");
            
            var appointment = await _appointmentService.GetAppointment(appointmentDto.AppointmentId, ct);

            if (appointment != null)
            {
                var detailsMessage = $"Детали записи:\n" +
                                        $"Процедура: {Helper.GetRussianSubtypeName(appointment.Procedure)}\n" +
                                        $"Дата: {appointment.AppointmentDate.ToString("dd.MM.yyyy")}\n" +
                                        $"Время: {appointment.AppointmentDate.ToString("HH:mm")}";

                await _messageService.EditMessageText(messageData.Chat, messageData.MessageId, detailsMessage, Keyboards.AppointmentListKeyboard(appointment), ct);
            }
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

            var scenarioResponse = await scenario.HandleMessageAsync(context, messageData.User, messageData.Chat, ct);

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



        #region МЕТОДЫ CALLBACK QUERY КОМАНД
        private async Task HandleDaySelectCommand(ScenarioContext context, MessageData messageData, CancellationToken ct)
        {
            if (context == null)
                return;

            var calendarDayCallbackDto = CalendarDayCallbackDto.FromString(messageData.UserInput);

            if (calendarDayCallbackDto.Date == default)
                return;

            context.Data["DateProcedure"] = calendarDayCallbackDto.Date;

            await ProcessScenario(context, messageData, ct);
        }

        private async Task HandleChangeMonthCommand(ScenarioContext context, MessageData messageData, string chooseMonth, CancellationToken ct)
        {
            if (context == null)
                return;

            var calendarMonthCallbackDto = CalendarMonthCallbackDto.FromString(messageData.UserInput);

            if (DateTime.TryParseExact(calendarMonthCallbackDto.Month, "MM", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime newDisplayMonth))
            {
                var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

                await _messageService.EditMessage(messageData.Chat, messageData.MessageId, Keyboards.DaySlotsKeyboard(newDisplayMonth, unavailableSlots), ct);
            }
            return;
        }

        private async Task HandleShowAppointmentsList(MessageData messageData, CancellationToken ct)
        {
            var pagedListDto = PagedListCallbackDto.FromString(messageData.UserInput);

            var activeAppointments = await GetKeyValuePairAppointmentsCollection(messageData.User.UserId, ct);

            var pagedKeyboard = await BuildPagedAppointmentButtons(activeAppointments, pagedListDto);

            await _messageService.EditMessageText(messageData.Chat, messageData.MessageId, "Список активных записей:", pagedKeyboard, ct);
        }

        private async Task HandleCancelAppointmentCommand(MessageData messageData, CancellationToken ct)
        {
            var appointmentDto = AppointmentCallbackDto.FromString(messageData.UserInput);

            if (appointmentDto.AppointmentId == null)
                throw new Exception("Беда с id записи");

            var appointment = await _appointmentService.GetAppointment(appointmentDto.AppointmentId, ct);

            var cancelContext = Helper.CreateScenarioContext(ScenarioType.CancelAppointment, messageData.TelegramUserId);

            cancelContext.Data["Appointment"] = appointment;

            await ProcessScenario(cancelContext, messageData, ct);
        }

        private async Task HandleApproveCancelAppointmentCommand(MessageData messageData, ScenarioContext context, CancellationToken ct)
        {
            if (!context.Data.TryGetValue("Appointment", out var appointment))
                throw new ArgumentException("Отменяемой записи - нет в контексте");

            if (appointment is not Appointment appointmentObj)
                throw new ArgumentException("Отменяемая запись - не запись");

            await _appointmentService.CancelAppointment(appointmentObj.Id, ct);
            
            await ProcessScenario(context, messageData, ct);
        }

        private async Task HandleDeclineCancelAppointmentCommand(MessageData messageData, CancellationToken ct)
        {
            await _scenarioContextRepository.ResetContext(messageData.TelegramUserId, ct);

            await _messageService.DeleteMessage(messageData.Chat, messageData.MessageId - 1, ct);

            await _messageService.SendMessage(messageData.Chat, "Будем с нетерпением ждать вас!\n Вы в главном меню. Что хотите сделать?\n", Keyboards.firstStep, ct);

            return;
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

