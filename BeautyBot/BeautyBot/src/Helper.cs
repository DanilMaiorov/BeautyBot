using BeautyBot.src.BeautyBot.Domain.Entities;
using System.Text;
using BeautyBot.src.BeautyBot.Core.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Globalization;
using BeautyBot.src.BeautyBot.TelegramBot.Scenario;
using BeautyBot.src.BeautyBot.Domain.Services;
using System.Security.Cryptography.X509Certificates;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;

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
                { Command.Main, "Записаться на процедуру" },
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
        public static (string, string, DateOnly) InputCheck(string input, IReadOnlyList<Appointment> currentUserAppointmentsList = null)
        {
            string cutInput = "";
            Guid taskGuid = Guid.Empty;

            string month = "";
            DateOnly date = default;

            var inputLower = input.ToLower();

            if (TimeOnly.TryParse(inputLower, out var time))
                inputLower = "/time";

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

                //тут кейсы даты и времени
                case string s when inputLower.StartsWith("day_selected_"):
                    date = ParseDateFromString(inputLower);
                    inputLower = "/date";
                    break;

                case string s when inputLower.StartsWith("prev_month_", StringComparison.OrdinalIgnoreCase) || inputLower.StartsWith("next_month_", StringComparison.OrdinalIgnoreCase):
                    month = GetFormattedMonth(inputLower);
                    inputLower = "/changemonth";
                    break;

                case "empty_day":
                case "month_display_no_action":
                case "day_name_no_action":
                    inputLower = "/date";
                    break;

                default:
                    break;
            }
            return (inputLower, month, date);
        }

        public static DateOnly ParseDateFromString(string input)
        {
            int lastUnderscoreIndex = input.LastIndexOf('_');

            if (lastUnderscoreIndex == -1 || lastUnderscoreIndex == input.Length - 1)
                throw new ArgumentException("Invalid input format");

            string dateString = input.Substring(lastUnderscoreIndex + 1);

            if (DateOnly.TryParseExact(dateString, "yyyy-MM-dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly result))
            {
                return result;
            }

            throw new FormatException("Could not parse date from string");
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
                await botClient.SendMessage(chat, $"{appointmentCounter}) ({appointment.State}) {appointment.Procedure.Name} - {appointment.CreatedAt}, {appointment.Id}", cancellationToken: ct);

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
        /// Кортеж, содержащий:
        /// <list type="bullet">
        ///     <item><term>Chat?</term><description>Объект чата, откуда пришло сообщение/колбэк (может быть null).</description></item>
        ///     <item><term>string?</term><description>Текстовый ввод пользователя (может быть null).</description></item>
        ///     <item><term>BeautyBotUser?</term><description>Объект пользователя (может быть null, если не найден).</description></item>
        /// </list>
        /// Возвращает кортеж из всех null-значений, если обновление не содержит сообщения или колбэка.
        /// </returns>
        public static async Task<(Chat?, string?, int, BeautyBotUser?)> HandleMessageAsyncGetData(Update update, ScenarioContext context, IUserService userService, CancellationToken ct)
        {
            Message? message;
            string? currentUserInput;
            int messageId;

            if (update.Message != null)
            {
                message = update.Message;
                messageId = update.Message.Id;
                currentUserInput = message.Text?.Trim();
            }
            else if (update.CallbackQuery != null)
            {
                message = update.CallbackQuery.Message;
                messageId = update.CallbackQuery.Message.Id;
                currentUserInput = update.CallbackQuery.Data.Trim();
            }
            else
            {
                return default;
            }

            var currentChat = message.Chat;
            var currentUser = await GetUserInScenario(context, currentChat.Id, currentChat.Username, userService, ct);

            return (currentChat, currentUserInput, messageId, currentUser);
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

        public static string GetBaseProcedureName(object procedureType)
        {
            if (procedureType is Manicure)
                return "Manicure";
            
            if (procedureType is Pedicure)
                return "Pedicure";
            
            throw new Exception("Неизвестный тип процедуры");
        }
    }
}
