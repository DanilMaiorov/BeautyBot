using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using Telegram.Bot.Polling;
using Telegram.Bot;
using Telegram.Bot.Types;
using BeautyBot.src.BeautyBot.Application.Services;

namespace BeautyBot.src.BeautyBot.TelegramBot.UpdateHandlers
{
    public delegate void MessageEventHandler(string message);
    public class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
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

            ICreateAppointmentService createAppointmentService,
            CancellationToken ct)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _procedureCatalogService = procedureCatalogService ?? throw new ArgumentNullException(nameof(procedureCatalogService));
            _priceCalculationService = priceCalculationService ?? throw new ArgumentNullException(nameof(priceCalculationService));

            _createAppointmentService = createAppointmentService;
            _ct = ct;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var currentChat = update.Message.Chat;

            string input = update.Message.Text;

            string eventMessage = "";


            CreateAppointmentTemplate currentStep = null;

            try
            {
                var currentUser = await _userService.GetUser(update.Message.From.Id, _ct);

                var currentUserTaskList = currentUser != null
                    ? await _appointmentService.GetUserAppointmentsByUserId(currentUser.UserId, _ct)
                    : null;

                if (update.Message.Id == 1)
                {
                    await botClient.SendMessage(currentChat, $"Привет! Это BeautyBot! \n", cancellationToken: _ct);
                    return;
                }

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

                (string inputCommand, string inputText, string appointmentDate, string appointmentTime, Guid taskGuid) = Helper.InputCheck(input, currentUserTaskList);

                input = inputCommand.Replace("/", string.Empty);



                //объявление типа данных команды
                Command command;

                if (Enum.TryParse<Command>(input, true, out var result))
                {
                    command = result;
                }
                else
                {
                    command = default;
                }



                //объявление типа данных типа маникюра
                ManicureType manicureType;

                if (Enum.TryParse<ManicureType>(input, true, out var type))
                {
                    manicureType = type;            
                }
                else
                {
                    manicureType = default;
                }


                Console.WriteLine(input);

                //КОНЕЦ ОБРАБОТКИ СООБЩЕНИЯ
                OnHandleUpdateCompleted?.Invoke(eventMessage);


                switch (manicureType)
                {
                    case ManicureType.None:
                        break;

                    default:
                        ProcedureFactory.CreateProcedure(input, out IProcedure procedure);

                        await _createAppointmentService.AddStep(procedure);

                        await botClient.SendMessage(currentChat, "Выберите дату", replyMarkup: Keyboards.chooseDate, cancellationToken: _ct);
                        return;
                }



                //проверка списка с шагами записи
                var steps = await _createAppointmentService.GetSteps();

                //if (steps.Count != 0)
                //{
                //    if (steps.Count == 1)
                //    {
                //        command = Command.Date;
                //    }
                //    else if (steps.Count == 2)
                //    {
                //        command = Command.Time;
                //    }
                //}


                //обработка основных команд
                switch (command)
                {
                    case Command.Start:
                        if (currentUser == null)
                        {
                            currentUser = await _userService.RegisterUser(update.Message.From.Id, update.Message.From.Username, _ct);
                        }
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
                        ProcedureFactory.CreateProcedure(inputText, out IProcedure procedure);
                        await _appointmentService.AddAppointment(currentUser, procedure, DateTime.Now, _ct);
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
                        await botClient.SendMessage(currentChat, "Выберите маникюр", replyMarkup: Keyboards.thirdStep, cancellationToken: _ct);
                        break;





                    case Command.Date:

                        currentStep = await _createAppointmentService.GetStep();

                        await botClient.SendMessage(currentChat, "Выберите время", replyMarkup: Keyboards.chooseTime, cancellationToken: _ct);


                        await _createAppointmentService.AddStep(currentStep.Procedure, appointmentDate);

                        await botClient.SendMessage(currentChat, "Выберите время", replyMarkup: Keyboards.chooseTime, cancellationToken: _ct);
                        break;
                    case Command.Time:

           

                        currentStep = await _createAppointmentService.GetStep();
                        await _createAppointmentService.AddStep(currentStep.Procedure, currentStep.AppointmentDate, appointmentTime);
                        break;

                    case Command.Back:
                        //currentStep = await _createAppointmentService.GetStep();
                        await botClient.SendMessage(currentChat, "Выберите маникюр", replyMarkup: Keyboards.thirdStep, cancellationToken: _ct);
                        return;




                    case Command.Approve:


                        break;


                    case Command.Exit:
                        //await ShowHelp(currentUser);
                        Console.WriteLine("ExitExit");
                        break;

                    default:
                        if (currentUser == null)
                            await botClient.SendMessage(currentChat, "Ошибка: введена некорректная команда. Пожалуйста, введите команду заново.\n", replyMarkup: Keyboards.start, cancellationToken: _ct);
                        //await Helper.CommandsRender(currentChat, botClient, _ct);
                        break;
                }
            }
            catch (Exception)
            {
                throw;
            }
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



        private async Task CreateAppointment()
        {

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

        //Task Otus.ToDoList.ConsoleBot.IUpdateHandler.HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        //{
        //    throw new NotImplementedException();
        //}



        //public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        //{
        // ... логика обработки сообщений ...

        //    if (update.Type == UpdateType.Message && update.Message?.Text != null)
        //    {
        //        var message = update.Message;
        //        var user = _userService.GetUser(message.From.Id);
        //        if (user == null)
        //        {
        //            user = _userService.RegisterUser(message.From.Id, message.From.Username ?? message.From.FirstName);
        //            // Отправить приветствие новому пользователю
        //        }

        //        if (message.Text == "/start")
        //        {
        //            // Отправить список доступных процедур
        //            var procedures = _procedureCatalogService.GetAllProcedures();
        //            // ... формирование ответа ...
        //        }
        //        else if (message.Text.StartsWith("/book"))
        //        {
        //            // Предположим, парсим ID процедуры из сообщения
        //            if (Guid.TryParse(message.Text.Replace("/book ", ""), out Guid procedureId))
        //            {
        //                var procedure = _procedureCatalogService.GetProcedureById(procedureId);
        //                if (procedure != null)
        //                {
        //                    // Пример: бронирование на текущее время (упрощенно)
        //                    var appointment = _appointmentService.BookAppointment(user.Id, procedure.Id, DateTime.Now.AddHours(1));

        //                    // Пример использования PriceCalculationService
        //                    var price = _priceCalculationService.CalculatePrice(new ProcedureCostDto(procedure));
        //                    // ... подтверждение бронирования с ценой ...
        //                }
        //            }
        //        }
        //        // ... другие команды ...
        //    }
        //}

        //public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        //{
        //    // ... логика обработки ошибок ...
        //}
    }
}
