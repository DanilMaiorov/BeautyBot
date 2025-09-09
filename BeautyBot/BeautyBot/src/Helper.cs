using BeautyBot.src.BeautyBot.Domain.Entities;
using System.Text;
using BeautyBot.src.BeautyBot.Core.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using BeautyBot.src.BeautyBot.TelegramBot.Scenario;
using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src
{
    public class Helper
    {
        /// <summary>
        /// Метод рендера списка команд
        /// </summary>
        /// <returns>Рендер списка в консоль (Телеграм)</returns>
        public async static Task CommandsRender(ITelegramBotClient botClient, CancellationToken ct)
        {
            int counter = 0;

            var builder = new StringBuilder();

            //команды бота
            var commands = new List<BotCommand>();

            //заведу словарик для описания команд
            var commandDescriptions = new Dictionary<Command, string>()
            {
                { Command.Start, "Начало работы с ботом, регистрация" },
                { Command.Help, "Помощь по командам" },
                { Command.Info, "Информация о боте" },
                { Command.Create, "Записаться на процедуру" },
                { Command.Show, "Показать текущие записи" },
            };

            builder.AppendLine("Список доступных команд:");

            foreach (Command commandValue in Enum.GetValues(typeof(Command)))
            {
                Command command = (Command)Enum.Parse(typeof(Command), commandValue.ToString());

                string commandName = $"/{commandValue.ToString().ToLower()}";

                if (commandDescriptions.TryGetValue(commandValue, out string? description))
                {
                    builder.AppendLine($"{++counter}) {commandName} - {description}");

                    commands.Add(new BotCommand { Command = commandName, Description = description });
                }
                else
                {
                    builder.AppendLine($"{++counter}) {commandName}");
                    commands.Add(new BotCommand { Command = commandName, Description = "" });
                }
            }

            await botClient.SetMyCommands(commands, cancellationToken: ct);
        }

        /// <summary>
        /// Метод форматирования ввода команды от пользователя
        /// </summary>
        /// <param name="input">Ввод пользователя</param>
        /// <returns>Форматированная строка</returns>
        public static string NormalizeInput(string input)
        {
            DateOnly date = default;

            var inputLower = input.ToLower();

            if (TimeOnly.TryParse(inputLower, out var time))
                inputLower = "время";

            switch (inputLower)
            {
                //основные
                case "старт":
                    inputLower = "/start";
                    break;

                case "записаться":
                    inputLower = "/create";
                    break;

                case "посмотреть текущие записи":
                    inputLower = "/show";
                    break;

                case "назад":
                    inputLower = "/back";
                    break;

                case "отмена":
                    inputLower = "/cancel";
                    break;

                //процедуры
                case "маникюр":
                    inputLower = "/manicure";
                    break;
                case "педикюр":
                    inputLower = "/pedicure";
                    break;
                case "классический":
                    inputLower = "/classic";
                    break;
                case "гель-лак":
                    inputLower = "/gelpolish";
                    break;
                case "френч":
                    inputLower = "/french";
                    break;

                case "верно":
                    inputLower = "/approve";
                    break;

                case "выбрать другое время":
                    inputLower = "/changetime";
                    break;
                case "выбрать другую дату":
                    inputLower = "/changedate";
                    break;

                case "время":
                    inputLower = "/time";
                    break;


                case "empty_day":
                case "month_display_no_action":
                case "day_name_no_action":
                case "day_unavailable":
                    inputLower = "/date";
                    break;

                default:
                    break;
            }

            return inputLower;
        }

        /// <summary>
        /// Нормализует название типа процедуры, преобразуя русскоязычные названия в английские идентификаторы.
        /// </summary>
        /// <param name="input">Входное название типа процедуры на русском языке.</param>
        /// <returns>Английский идентификатор типа процедуры или исходное значение, если соответствие не найдено.</returns>
        public static string NormalizeProcedureTypeName(string input)
        {
            var lowerInput = input.ToLower();

            switch (lowerInput)
            {
                case "гель-лак":
                    return "GelPolish";
                case "классический":
                    return "Classic";
                case "френч":
                    return "French";
                default:
                    return input;
            }
        }

        /// <summary>
        /// Парсит строку в значение указанного enum, игнорируя регистр. 
        /// Возвращает default, если парсинг не удался или строка пустая.
        /// </summary>
        /// <typeparam name="T">Тип enum</typeparam>
        /// <param name="input">Строка для парсинга</param>
        /// <returns>Значение enum или default</returns>
        public static T GetEnumValueOrDefault<T>(string input) where T : struct, Enum
        {
            return Enum.TryParse<T>(input, true, out var result)
                ? result
                : default;
        }

        /// <summary>
        /// Фабричный метод для создания контекста сценария
        /// </summary>
        /// <param name="type">Тип создаваемого сценария</param>
        /// <returns>Новый экземпляр ScenarioContext</returns>
        public static ScenarioContext CreateScenarioContext(ScenarioType type, long userId)
        {
            return new ScenarioContext(type, userId);
        }

        /// <summary>
        /// Извлекает ключевые данные из входящего обновления (Update) от Telegram,
        /// такие как текущий чат, пользовательский ввод и информацию о пользователе.
        /// </summary>
        /// <param name="update">Объект Update, содержащий информацию о сообщении или колбэке.</param>
        /// <param name="context">Контекст сценария, используемый для получения данных пользователя.</param>
        /// <param name="userService">Сервис для получения информации о пользователе, если он отсутствует в контексте.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>
        /// Объект типа MessageData, содержащий:
        /// <list type="bullet">
        ///     <item><term>Chat?</term><description>Объект чата, откуда пришло сообщение/колбэк (может быть null).</description></item>
        ///     <item><term>string?</term><description>Текстовый ввод пользователя (может быть null).</description></item>
        ///     <item><term>BeautyBotUser?</term><description>Объект пользователя (может быть null, если не найден).</description></item>
        /// </list>
        /// Возвращает null, если обновление не содержит сообщения или колбэка.
        /// </returns>
        public static async Task<MessageData?> HandleMessageAsyncGetData(Update update, IUserService userService, CancellationToken ct)
        {
            if (update.Message != null)
                return new MessageData(
                    update.Message.Chat,
                    update.Message.Text?.Trim(),
                    update.Message.Id,
                    await userService.GetUser(update.Message.From.Id, ct)
                    );

            else if (update.CallbackQuery != null)
                return new MessageData(
                    update.CallbackQuery.Message.Chat,
                    update.CallbackQuery.Data?.Trim(),
                    update.CallbackQuery.Message.Id,
                    await userService.GetUser(update.CallbackQuery.From.Id, ct)
                    );

            else
                return null;
        }

        /// <summary>
        /// Пытается получить объект пользователя (BeautyBotUser) из данных контекста сценария.
        /// Если пользователь не найден в контексте или имеет неподходящий тип,
        /// асинхронно получает его из сервиса пользователей.
        /// </summary>
        /// <param name="context">Контекст сценария, содержащий данные.</param>
        /// <param name="id">Идентификатор пользователя для получения из сервиса, если не найден в контексте.</param>
        /// <param name="username">Имя пользователя для поиска в данных контекста.</param>
        /// <param name="ct">Токен отмены операции.</param>
        /// <returns>Найденный объект BeautyBotUser или null, если пользователь не найден ни в контексте, ни в сервисе.</returns>
        public static async Task<BeautyBotUser?> GetUserInScenario(ScenarioContext context, long id, string username, IUserService userService, CancellationToken ct)
        {
            if (context?.Data.TryGetValue(username, out var dataObject) == true && dataObject is BeautyBotUser beautyBotUser)
                return beautyBotUser;

            return await userService.GetUser(id, ct);
        }

        /// <summary>
        /// Возвращает строковое представление подтипа процедуры на основе её фактического типа.
        /// </summary>
        /// <param name="entity">Объект IProcedure, чей подтип необходимо получить.</param>
        /// <returns>Строковое представление типа процедуры (например, "French" или "Classic").</returns>
        /// <exception cref="Exception">Выбрасывается, если тип процедуры неизвестен или не поддерживается.</exception>
        public static string GetSubtypeName(IProcedure entity)
        {
            if (entity is Manicure manicure)
                return manicure.Type.ToString();

            if (entity is Pedicure pedicure)
                return pedicure.Type.ToString();

            throw new Exception("Неизвестный тип процедуры");
        }

        /// <summary>
        /// Форматирует информацию о процедуре и дате/времени в читаемую строку на русском языке.
        /// </summary>
        /// <param name="procedure">Объект IProcedure, содержащий информацию о типе и подтипе процедуры.</param>
        /// <param name="appointmentDateTime">Объект DateTime с датой и временем записи.</param>
        /// <returns>Строка в формате "Базовый_Тип (Подтип) - День Месяц в Время", например, "Маникюр (френч) - 21 Сентябрь в 15:00".</returns>
        public static string FormatAppointmentString(IProcedure procedure, DateTime appointmentDateTime)
        {
            string baseTypeName = GetRussianBaseTypeName(procedure);

            string subtypeName = GetRussianSubtypeName(procedure);

            string dateString = appointmentDateTime.ToString("dd.MM");

            string timeString = appointmentDateTime.ToString("HH:mm");

            //собираю итоговую строку
            return $"{baseTypeName} ({subtypeName}) - {dateString} в {timeString}";
        }

        /// <summary>
        /// Форматирует детали записи в строку для отображения.
        /// </summary>
        /// <param name="appointment">Запись для форматирования.</param>
        /// <returns>Строка с деталями записи.</returns>
        public static string FormatAppointmentDetails(Appointment appointment)
        {
            return $"Детали записи:\n" +
                   $"Процедура: {GetRussianBaseTypeName(appointment.Procedure) + ", " + GetRussianSubtypeName(appointment.Procedure)}\n" +
                   $"Дата: {appointment.AppointmentDate:dd.MM.yyyy}\n" +
                   $"Время: {appointment.AppointmentDate:HH:mm}";
        }

        /// <summary>
        /// Возвращает строковое значение базового типа процедуры.
        /// </summary>
        /// <param name="procedure">Процедура для получения базового типа.</param>
        /// <returns>Строковое представление базового типа процедуры.</returns>
        public static string GetRussianBaseTypeName(IProcedure procedure)
        {
            switch (procedure.BaseType)
            {
                case ProcedureBaseType.Manicure:
                    return Constants.Manicure;
                case ProcedureBaseType.Pedicure:
                    return Constants.Manicure; // Опечатка? Должно быть Constants.Pedicure?
                default:
                    return procedure.BaseType.ToString();
            }
        }

        /// <summary>
        /// Возвращает русское название подтипа процедуры.
        /// </summary>
        /// <param name="procedure">Процедура для получения названия.</param>
        /// <returns>Название подтипа на русском языке или пустая строка.</returns>
        public static string GetRussianSubtypeName(IProcedure procedure)
        {
            switch (procedure)
            {
                case Manicure manicure:
                    switch (manicure.Type)
                    {
                        case ManicureType.French:
                            return Constants.FrenchManicure;
                        case ManicureType.GelPolish:
                            return Constants.GelPolishManicure;
                        case ManicureType.Classic:
                            return Constants.ClassicManicure;
                        default:
                            return manicure.Type.ToString();
                    }

                case Pedicure pedicure:
                    switch (pedicure.Type)
                    {
                        case PedicureType.French:
                            return Constants.FrenchPedicure;
                        case PedicureType.GelPolish:
                            return Constants.GelPolishPedicure;
                        case PedicureType.Classic:
                            return Constants.ClassicPedicure;
                        default:
                            return pedicure.Type.ToString();
                    }

                default:
                    return string.Empty;
            }
        }
    }
}
