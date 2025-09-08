using BeautyBot.src.BeautyBot.Domain.Entities;
using System.Text;
using BeautyBot.src.BeautyBot.Core.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.Globalization;
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
        public async static Task CommandsRender(Chat chat, ITelegramBotClient botClient, CancellationToken ct)
        {
            int counter = 0;

            //создам стрингБилдер для сборки в одно сообщение, а не пачки
            var builder = new StringBuilder();

            //команды бота
            //var commands = new List<BotCommand>();

            //заведу словарик для описания команд
            var commandDescriptions = new Dictionary<Command, string>()
            {
                { Command.Start, "Начало работы с ботом, регистрация" },
                { Command.Help, "Помощь по командам" },
                { Command.Info, "Информация о боте" },
                //{ Command.AddAppointment, "Записаться на процедуру" },
                //{ Command.Main, "Записаться на процедуру" },
                { Command.Show, "Показать все записи" },
                { Command.CancelAppointment, "Отменить запись" },
                { Command.FindAppointment, "Найти запись" },
                { Command.EditAppointment, "Изменить запись" },
                { Command.Report, "Показать статистику записей" },
                { Command.Exit, "Выйти" }
            };

            builder.AppendLine("Список доступных команд:");

            foreach (Command commandValue in Enum.GetValues(typeof(Command)))
            {
                //Command command = (Commands)Enum.Parse(typeof(Commands), commandValue.ToString());

                string commandName = $"/{commandValue.ToString().ToLower()}";

                if (commandDescriptions.TryGetValue(commandValue, out string? description))
                {
                    builder.AppendLine($"{++counter}) {commandName} - {description}");

                    //commands.Add(new BotCommand { Command = commandName, Description = description });
                }
                else
                {
                    builder.AppendLine($"{++counter}) {commandName}");
                    //commands.Add(new BotCommand { Command = commandName, Description = "" });
                }
            }

            await botClient.SendMessage(chat, builder.ToString(), cancellationToken: ct);

            //рендерю менюшку
            //await botClient.SetMyCommands(commands, cancellationToken: ct);
        }

        /// <summary>
        /// Метод форматирования ввода команды от пользователя /add, /removetask, /completetask, /find
        /// </summary>
        /// <param name="input">Ввод пользователя</param>
        /// <param name="currentUserAppointmentsList">Список записей юзера</param>
        /// <returns>Кортеж с данными по записи</returns>
        public static (string, string, DateOnly) NormalizeInput(string input, IReadOnlyList<Appointment> currentUserAppointmentsList = null)
        {
            Guid taskGuid = Guid.Empty;

            string month = "";
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


                //case string s when inputLower.StartsWith("day_selected_"):
                //    date = ParseDateFromString(inputLower);
                //    inputLower = "/date";
                //    break;

                case string s when inputLower.StartsWith("prev_month_", StringComparison.OrdinalIgnoreCase) || inputLower.StartsWith("next_month_", StringComparison.OrdinalIgnoreCase):
                    month = GetFormattedMonth(inputLower);
                    inputLower = "/changemonth";
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

            return (inputLower, month, date);
        }

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

        public static string GetFormattedMonth(string callbackData)
        {
            if (!callbackData.StartsWith("prev_month_", StringComparison.OrdinalIgnoreCase) &&
                !callbackData.StartsWith("next_month_", StringComparison.OrdinalIgnoreCase)
                )
            {
                return "Неверный формат данных";
            }

            var dateFromCallback = callbackData.Split("_");

            var formattedDate = "";

            if (DateTime.TryParseExact(dateFromCallback[dateFromCallback.Length - 1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                switch (dateFromCallback[0])
                {
                    case "next":
                    case "prev":
                        formattedDate = date.ToString("MM", CultureInfo.CurrentCulture);
                        break;
                    default:
                        break;
                }

                return formattedDate;
            }
            else
            {
                return "Ошибка парсинга даты";
            }
        }

        /// <summary>
        /// Метод рендера списка записей на процедуры
        /// </summary>
        /// <param name="appointments">Список записей пользователя</param>
        /// <param name="botClient">Бот</param>
        /// <param name="chat">Номер чата</param>
        /// <param name="ct">Токен отмены</param>
        public async static Task AppointmentsListRender(IReadOnlyList<Appointment> appointments, ITelegramBotClient botClient, Chat chat, CancellationToken ct)
        {
            int appointmentCounter = 0;

            var builder = new StringBuilder();

            foreach (Appointment appointment in appointments)
            {
                appointmentCounter++;
                //await botClient.SendMessage(chat, $"{appointmentCounter}) ({appointment.State}) {appointment.Procedure.Name} - {appointment.CreatedAt}, {appointment.Id}", cancellationToken: ct);

                //тут надо попробовать выводить запись вместе с кнопками разными сообщениями
                //await botClient.SendMessage(chat, $"\n{appointment.Id}", ct);
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
            string baseTypeName;

            switch (procedure.BaseType)
            {
                case ProcedureBaseType.Manicure:
                    baseTypeName = Constants.Manicure;
                    break;
                case ProcedureBaseType.Pedicure:
                    baseTypeName = Constants.Manicure;
                    break;
                default:
                    baseTypeName = procedure.BaseType.ToString();
                    break;
            }
            string subtypeName = GetRussianSubtypeName(procedure);

            string dateString = appointmentDateTime.ToString("dd.MM");

            string timeString = appointmentDateTime.ToString("HH:mm");

            //собираю итоговую строку
            return $"{baseTypeName} ({subtypeName}) - {dateString} в {timeString}";
        }

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
