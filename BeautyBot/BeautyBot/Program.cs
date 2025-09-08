using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using BeautyBot.src.BeautyBot.Application.Services;
using BeautyBot.src.BeautyBot.TelegramBot.UpdateHandlers;
using BeautyBot.src.BeautyBot.Domain.Entities.Repositories;
using Telegram.Bot;
using Telegram.Bot.Polling;
using BeautyBot.src.BeautyBot.TelegramBot.Scenario;
using BeautyBot.src.BeautyBot.Core.DataAcess.Context;
using BeautyBot.src.BeautyBot.Core.DataAcess.Database;
using LinqToDB;

namespace BeautyBot
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            string botToken = Environment.GetEnvironmentVariable("BeautyBot_token", EnvironmentVariableTarget.User);
            string connectionString = Environment.GetEnvironmentVariable("ConnectionStringPostgreSQL_BeautyBot", EnvironmentVariableTarget.User);
            string providerName = ProviderName.PostgreSQL;


            if (string.IsNullOrEmpty(botToken) || string.IsNullOrEmpty(connectionString))
            {
                if (string.IsNullOrEmpty(botToken))
                    Console.WriteLine("Bot token not found. Please set the environment variable.");

                if (string.IsNullOrEmpty(connectionString))
                    Console.WriteLine("Connection string not found. Please set the environment variable.");
            }

            var botClient = new TelegramBotClient(botToken);

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = []
            };

            using var cts = new CancellationTokenSource();

            //POSTGRESQL
            IDataContextFactory<BeautyBotDataContext> factory = new DataContextFactory(connectionString, providerName);

            //Инициализация БД
            DatabaseInitializer databaseInitializer = new DatabaseInitializer(factory);

            databaseInitializer.Initialize();


            IUserRepository userRepository = new PostgreSqlUserRepository(factory);
            ISlotRepository slotRepository = new PostgreSqlSlotRepository(factory);
            IAppointmentRepository appointmentRepository = new PostgreSqlAppointmentRepository(factory);

            IUserService _userService = new UserService(userRepository);
            ISlotService _slotService = new SlotService(slotRepository);

            IMessageService _messageService = new MessageService(botClient);

            PostgreSqlProcedureRepository procedureRepository = new PostgreSqlProcedureRepository(factory);

            //Задача написать фичу, которая продлевает количество слотов
            await _slotService.GenerateYearlySlots(cts.Token);

            IAppointmentService _appointmentService = new AppointmentService(appointmentRepository);

            //добавить сервис для отчета
            //IToDoReportService _toDoReportService = new ToDoReportService(toDoRepository);

            //логика сценариев
            IScenarioContextRepository _contextRepository = new InMemoryScenarioContextRepository();
            IEnumerable<IScenario> _scenarios = new List<IScenario>
            {
                new AddAppointmentScenario(_appointmentService, _slotService, procedureRepository),
                new CancelAppointmentScenario(_appointmentService, _slotService, procedureRepository),
            };

            IUpdateHandler _updateHandler = new UpdateHandler(
                _userService,
                _appointmentService,
                procedureRepository,

                _slotService,

                _messageService,

                _scenarios,
                _contextRepository,

                cts.Token);

            if (_updateHandler is UpdateHandler castHandler)
            {
                //подписываюсь на события
                castHandler.OnHandleUpdateStarted += castHandler.HandleStart;
                castHandler.OnHandleUpdateCompleted += castHandler.HandleComplete;

                try
                {
                    botClient.StartReceiving(
                    _updateHandler,
                        receiverOptions: receiverOptions,
                        cancellationToken: cts.Token
                    );

                    //запускаю цикл, которые будет работать пока не нажму А
                    while (true)
                    {
                        Console.WriteLine("Нажми A и Ввод для остановки и выхода из бота");
                        var s = Console.ReadLine();

                        if (s?.ToUpper() == "A")
                        {
                            cts.Cancel();
                            Console.WriteLine("Бот остановлен");
                            break;
                        }
                        var me = await botClient.GetMe();
                        Console.WriteLine($"{me.FirstName} запущен!");
                    }
                }
                finally
                {
                    //отписываюсь от событий
                    castHandler.OnHandleUpdateStarted -= castHandler.HandleStart;
                    castHandler.OnHandleUpdateCompleted -= castHandler.HandleComplete;
                    Console.WriteLine("finally");
                }
            }
        }
    }
}
