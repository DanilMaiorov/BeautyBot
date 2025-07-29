using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Application.Services;
//using BeautyBot.src.BeautyBot.Infrastructure.Repositories.Json;
using BeautyBot.src.BeautyBot.TelegramBot.UpdateHandlers;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Entities.Repositories;
using Telegram.Bot;
using Telegram.Bot.Polling;

namespace BeautyBot
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            //string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN", EnvironmentVariableTarget.User);
            string token = "8389262050:AAGxiMOnoOzcgrytZfmSnewL-PXFkv2fp38";

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("Bot token not found. Please set the BEAUTY_BOT_TOKEN environment variable.");
                return;
            }

            var botClient = new TelegramBotClient(token);

            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = []
            };

            using var cts = new CancellationTokenSource();

            IUserRepository userRepository = new InMemoryUserRepository();

            IAppointmentRepository appointmentRepository = new InMemoryAppointmentRepository();
            //IAppointmentRepository appointmentRepository = new InMemoryAppointmentRepository(); ТУТ НУЖНО БУДЕТ ЗАМЕНИТЬ НА ХРАНЕНИЕ В ЛОКАЛЬНЫХ ФАЙЛАХ

            IProcedureDefinitionRepository procedureDefinitionRepository = new InMemoryProcedureDefinitionRepository();
            ISlotRepository slotRepository = new InMemorySlotRepository();

            IUserService _userService = new UserService(userRepository);

            IAppointmentService _appointmentService = new AppointmentService(appointmentRepository, procedureDefinitionRepository, slotRepository);

            ISlotService _slotService = new SlotService(slotRepository);




            IProcedureCatalogService _procedureCatalogService = new ProcedureCatalogService(procedureDefinitionRepository);
            IPriceCalculationService _priceCalculationService = new PriceCalculationService();

            //добавить сервис для отчета
            //IToDoReportService _toDoReportService = new ToDoReportService(toDoRepository);


            //ДЛЯ СОЗДАНИЯ ЗАПИСИ
            ICreateAppointmentTemplate createAppointmentTemplate = new InMemoryCreateAppointmentTemplate();

            ICreateAppointmentService _createAppointmentService = new CreateAppointmentService(createAppointmentTemplate);





            IUpdateHandler _updateHandler = new UpdateHandler(
                _userService,
                _appointmentService,
                _procedureCatalogService,
                _priceCalculationService,

                _slotService,
                _createAppointmentService,
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

            //IProcedure proc = new GelPolishManicure();

            //IProcedure proc2 = new FrenchManicure();
            //void ManLog(IProcedure procedure)
            //{
            //    Console.WriteLine(procedure.Name);
            //    Console.WriteLine(procedure.Price);
            //}

            //ManLog(proc);
            //ManLog(proc2);
            //работает!


            // В Program.cs (для .NET 6+) или Startup.cs
            //var builder = WebApplication.CreateBuilder(args);

            //// Регистрация репозиториев
            //builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            //builder.Services.AddSingleton<IProcedureDefinitionRepository, InMemoryProcedureDefinitionRepository>();
            //builder.Services.AddSingleton<IAppointmentRepository, InMemoryAppointmentRepository>();

            //// Регистрация сервисов
            //builder.Services.AddSingleton<IUserService, UserService>();
            //builder.Services.AddSingleton<IAppointmentService, AppointmentService>();
            //builder.Services.AddSingleton<IProcedureCatalogService, ProcedureCatalogService>();
            //builder.Services.AddSingleton<IPriceCalculationService, PriceCalculationService>();

            //// Регистрация хэндлера
            //builder.Services.AddTransient<UpdateHandler>(); // Transient, если он не хранит состояние

            //// ... остальная часть настройки бота и хостинга ...

            //var app = builder.Build();

            //// Получение UpdateHandler из ServiceProvider для запуска бота
            //using (var scope = app.Services.CreateScope())
            //{
            //    var updateHandler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();
            //    var botClient = // ... ваш TelegramBotClient ...

            //    // Настройка получения обновлений
            //    botClient.StartReceiving(updateHandler.HandleUpdateAsync, updateHandler.HandleErrorAsync);
            //}


            //Четкое разделение ответственности: Каждый класс и сервис занимается своей конкретной задачей.
            //Гибкость в определении процедур: Легко добавлять новые виды процедур, расширять их функционал, не затрагивая основной код.
            //Изоляция данных: Каждая сущность(пользователи, определения процедур, записи) имеет свой репозиторий, что упрощает управление данными и их миграцию.
            //Тестируемость: Каждый компонент можно тестировать изолированно, мокая его зависимости.
            //Масштабируемость: Хотя вы пока используете In - Memory / JSON, такая структура идеально подходит для перехода к реальным базам данных(SQL, NoSQL) или даже к разделению на реальные микросервисы в будущем.Замена репозитория на базу данных не потребует изменений в сервисах или хэндлере.
            //Абстракция: Использование интерфейсов(IProcedure, IProcedureCost, IRepository) позволяет легко менять реализации без изменения кода, который их использует.

            //BeautyBot /
            //├── .git /
            //├── .github /              # Для CI/CD, GitHub Actions (необязательно)
            //├── src /
            //│   ├── BeautyBot.Core /   # Общие определения, базовые классы, контракты
            //│   ├── BeautyBot.Domain / # Бизнес-логика, доменные модели
            //│   ├── BeautyBot.Infrastructure / # Реализации репозиториев, внешние сервисы
            //│   ├── BeautyBot.Application / # Сервисы приложения, Use Cases
            //│   ├── BeautyBot.Bot /    # Основная логика бота, UpdateHandler, Program.cs
            //│   └── BeautyBot.Tests /  # Тесты для всех слоев
            //├── config /               # Файлы конфигурации (например, appsettings.json)
            //├── data /                 # Папка для JSON-файлов (если хранение в JSON)
            //├── Dockerfile            # Для контейнеризации (необязательно)
            //├── README.md
            //└── BeautyBot.sln         # Файл решения Visual Studio


            //BeautyBot.Core / (Общие контракты и базовые классы)
            //Этот проект будет содержать всё, что является общим для многих частей системы и не содержит бизнес - логики.
            //Его задача — быть "тощим" и не зависеть от других слоев, кроме базовых типов .NET.

            //BeautyBot.Core /
            //├── Interfaces /
            //│   ├── IRepository.cs        # Общий интерфейс IRepository<T>
            //│   ├── IProcedure.cs         # IProcedure, IProcedureCost
            //│   └── IBaseEntity.cs        # Если у вас есть общий базовый интерфейс для сущностей с Id
            //├── Enums /
            //│   ├── AppointmentStatus.cs
            //│   └── UserRole.cs           # Если есть роли пользователей
            //├── BaseClasses /
            //│   └── ProcedureBase.cs      # Абстрактный базовый класс ProcedureBase
            //└── Dtos /                     # Простые DTO (Data Transfer Objects), если нужны
            //    └── ProcedureCostDto.cs


            //BeautyBot.Domain / (Доменные модели и бизнес - правила)
            //Здесь находится суть вашего бизнеса.Это ваши доменные сущности(не просто DTO, а объекты с поведением) и их логика.

            //BeautyBot.Domain /
            //├── Entities /                 # Сами доменные сущности
            //│   ├── ToDoUser.cs           # Переименуйте в User.cs
            //│   ├── Appointment.cs
            //│   ├── Manicure.cs
            //│   ├── GelPolishManicure.cs
            //│   ├── FrenchManicure.cs
            //│   ├── Brows.cs
            //│   └── Lashes.cs
            //├── Services /                 # Интерфейсы доменных сервисов (чистые, без реализации)
            //│   ├── IUserService.cs       # Интерфейс
            //│   ├── IAppointmentService.cs
            //│   ├── IProcedureCatalogService.cs
            //│   └── IPriceCalculationService.cs
            //└── Repositories /             # Интерфейсы репозиториев, специфичные для домена
            //    ├── IUserRepository.cs
            //    ├── IProcedureDefinitionRepository.cs
            //    └── IAppointmentRepository.cs

            //Замечание: Иногда интерфейсы сервисов и репозиториев располагают в BeautyBot.Application, 
            //но для более строгого DDD - подхода их можно разместить здесь.


            //BeautyBot.Infrastructure / (Реализации репозиториев и внешних сервисов)
            //Этот слой отвечает за взаимодействие с внешним миром – базами данных, файлами, сторонними API.
            //Здесь находятся конкретные реализации интерфейсов из BeautyBot.Domain.

            //BeautyBot.Infrastructure /
            //├── Repositories /
            //│   ├── InMemory /             # In-memory реализации
            //│   │   ├── InMemoryUserRepository.cs
            //│   │   ├── InMemoryProcedureDefinitionRepository.cs
            //│   │   └── InMemoryAppointmentRepository.cs
            //│   └── Json /                 # JSON реализации (когда перейдете на них)
            //│       ├── JsonUserRepository.cs
            //│       ├── JsonProcedureDefinitionRepository.cs
            //│       └── JsonAppointmentRepository.cs
            //├── ThirdPartyServices /       # Если будете использовать сторонние API (например, для платежей)
            //│   └── PaymentGatewayService.cs
            //└── Persistence /              # Контексты БД, если будете использовать EF Core
            //    └── AppDbContext.cs

            //BeautyBot.Application / (Сервисы приложения / Use Cases)
            //Этот слой содержит бизнес-логику, которая координирует работу доменных сущностей и репозиториев.
            //Он отвечает за сценарии использования(Use Cases) приложения.Здесь будут находиться реализации 
            //интерфейсов сервисов, которые вы определили в домене.

            //BeautyBot.Application /
            //├── Services /
            //│   ├── UserService.cs        # Реализация IUserService
            //│   ├── AppointmentService.cs # Реализация IAppointmentService
            //│   ├── ProcedureCatalogService.cs # Реализация IProcedureCatalogService
            //│   └── PriceCalculationService.cs # Реализация IPriceCalculationService
            //└── Mappers /                  # Если нужны мапперы между сущностями и DTO
            //    └── AppointmentMapper.cs
            //BeautyBot.Bot / (Основное приложение бота)
            //Это точка входа вашего приложения.Здесь происходит инициализация, настройка DI-контейнера и взаимодействие с Telegram API.

            //BeautyBot.Bot /
            //├── UpdateHandlers /
            //│   └── UpdateHandler.cs      # Ваш основной обработчик
            //├── TelegramBotClientFactory.cs # Если создаете клиента через фабрику
            //├── Program.cs                # Точка входа, настройка DI
            //└── appsettings.json          # Настройки бота (токен и т.д.)
            //BeautyBot.Tests / (Проект для тестов)
            //Обязательно выделите отдельный проект для тестов.

            //BeautyBot.Tests /
            //├── UnitTests /
            //│   ├── DomainTests /
            //│   │   ├── UserTests.cs
            //│   │   └── AppointmentTests.cs
            //│   ├── ApplicationTests /
            //│   │   ├── UserServiceTests.cs
            //│   │   └── AppointmentServiceTests.cs
            //│   └── InfrastructureTests /
            //│       ├── InMemoryUserRepositoryTests.cs
            //│       └── JsonUserRepositoryTests.cs
            //└── IntegrationTests /
            //    └── BotIntegrationTests.cs
            //Почему такая структура?
            //Принцип наименьших зависимостей (Dependency Rule): Внешние круги зависят от внутренних, но не наоборот.
            //BeautyBot.Bot и BeautyBot.Infrastructure зависят от BeautyBot.Application.
            //BeautyBot.Application зависит от BeautyBot.Domain и BeautyBot.Core.
            //BeautyBot.Domain зависит только от BeautyBot.Core.
            //BeautyBot.Core ни от чего не зависит, кроме базовых библиотек .NET.
            //Чистая архитектура (Clean Architecture / Onion Architecture): Эта структура соответствует принципам чистой архитектуры, 
            //делая ваше приложение более гибким, тестируемым и независимым от внешних деталей реализации.
            //Масштабируемость: Когда вы решите выделить часть функционала в отдельный микросервис (например, сервис для управления расписанием 
            //или платежами), вы сможете легко вырезать соответствующий домен, его сервисы и инфраструктуру, и перенести в новый репозиторий.
            //Разделение ответственности: Каждая папка или проект имеет четко определенную роль, что упрощает понимание структуры проекта.
            //Начните с этой структуры, и по мере роста проекта вы сможете адаптировать ее под свои нужды, но основы, заложенные здесь, обеспечат прочную базу.



            //            Вот почему DI является ключевым элементом предложенной архитектуры:

            //Интерфейсы как контракты: Я везде предложил использовать интерфейсы(например, IUserService, IAppointmentService, IUserRepository, IProcedure).Это фундаментальный принцип DI: вы программируете к интерфейсам, а не к конкретным реализациям.

            //Конструкторная инъекция(Constructor Injection): В каждом сервисе(UserService, AppointmentService, PriceCalculationService и даже UpdateHandler) зависимости передаются через конструктор.
            //Например, в AppointmentService:

            //C#

            //public class AppointmentService : IAppointmentService
            //        {
            //            private readonly IAppointmentRepository _appointmentRepository;
            //            private readonly IProcedureDefinitionRepository _procedureDefinitionRepository;

            //            public AppointmentService(IAppointmentRepository appointmentRepository, IProcedureDefinitionRepository procedureDefinitionRepository)
            //            {
            //                _appointmentRepository = appointmentRepository;
            //                _procedureDefinitionRepository = procedureDefinitionRepository;
            //            }
            //            // ...
            //        }
            //        Это означает, что AppointmentService не заботится о том, как создать экземпляры IAppointmentRepository и IProcedureDefinitionRepository.Он просто заявляет, что ему они нужны.


            //        Контейнер DI (Inversion of Control Container): Я явно указал, как регистрировать зависимости в DI-контейнере(на примере встроенного контейнера .NET Core в Program.cs) :

            //C#

            //// Регистрация репозиториев
            //builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            //        // ...

            //        // Регистрация сервисов
            //        builder.Services.AddSingleton<IUserService, UserService>();
            //// ...
            //Этот код инструктирует DI-контейнер:

            //Когда кто-то запрашивает IUserRepository, дай ему экземпляр InMemoryUserRepository.
            //Когда кто-то запрашивает IUserService, дай ему экземпляр UserService (и попутно создай для него все зависимости, которые он просит через конструктор, например IUserRepository).
            //Разделение ответственности и тестируемость: DI позволяет легко подменять реализации зависимостей.

            //В продакшене вы используете InMemory (или Json, или EFCore) репозитории.
            //В тестах вы можете легко "замокать" (mock) эти репозитории, предоставив фейковые реализации или используя фреймворки для мокирования (например, Moq), чтобы проверить только логику самого сервиса, а не его зависимости.
            //Гибкость: Если вы решите перейти от InMemory хранения к JSON или к реальной базе данных (например, PostgreSQL через Entity Framework Core), вам нужно будет изменить только одну строку в конфигурации DI-контейнера(builder.Services.AddSingleton<IUserRepository, JsonUserRepository>() или builder.Services.AddScoped<IUserRepository, EfCoreUserRepository>()), а не переписывать код, который использует IUserRepository.

            //Таким образом, DI является центральным элементом предложенной архитектуры, обеспечивая ее модульность, тестируемость, гибкость и масштабируемость.Без DI такая структура была бы крайне неудобной в управлении и развитии.
        }
    }
}
