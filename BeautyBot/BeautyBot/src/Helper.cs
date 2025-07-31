using BeautyBot.src.BeautyBot.Domain.Entities;
using System.Text;
using BeautyBot.src.BeautyBot.Core.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using System.Globalization;

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
                { Command.Add, "Записаться на процедуру" },
                { Command.ShowActiveAppointments, "Показать активные записи" },
                { Command.ShowAllAppointments, "Показать все записи" },
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
        public static (string, string, string, DateOnly, TimeOnly, Guid) InputCheck(string input, IReadOnlyList<Appointment> currentUserAppointmentsList = null)
        {
            string cutInput = "";
            Guid taskGuid = Guid.Empty;

            string month = "";
            DateOnly date = default;

            var inputLower = input.ToLower();

            if (inputLower.StartsWith("day_selected_"))
            {
                date = ParseDateFromString(inputLower);
                inputLower = "/date";
            } 
            else if (inputLower.StartsWith("prev_month_", StringComparison.OrdinalIgnoreCase) || inputLower.StartsWith("next_month_", StringComparison.OrdinalIgnoreCase))
            {
                month = GetFormattedMonth(inputLower);
                inputLower = "/changemonth";

            }
            //// Обработка других кнопок (дни недели, пустые дни, отключенные дни, отображение месяца)
            //else
            //{
            //    // Просто закрываем всплывающее уведомление, так как эти кнопки не требуют других действий
            //    await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
            //}



            if (TimeOnly.TryParse(inputLower, out var time))
                inputLower = "/time";
            



            switch (inputLower)
            {
                case "старт":
                    inputLower = "/start";
                    break;

                case "записаться":
                    inputLower = "/create";
                    break;



                case "маникюр":
                    inputLower = "/manicure";
                    break;

                case "педикюр":
                    inputLower = "/pedicure";
                    break;



                case "френч":
                    inputLower = "/french";
                    break;

                case "гель-лак":
                    inputLower = "/gelPolish";
                    break;

                case "классический":
                    inputLower = "/classic";
                    break;




                case "выбрать другую дату":
                    inputLower = "/changedate";
                    break;



                case "выбрать другое время":
                    inputLower = "/changetime";
                    break;


                case "верно":
                    inputLower = "/approve";
                    break;

                case "назад":
                    inputLower = "/back";
                    break;


                default:
                    break;
            }
            return (inputLower, cutInput, month, date, time, taskGuid);
        }


        private static DateOnly ParseDateFromString(string input)
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

        private static DateOnly ParseTimeFromString(string input)
        {

            if (DateOnly.TryParseExact(input, "yyyy-MM-dd",
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






        // метод валидации задачи
        public static (string, Guid) Validate(string input, Guid taskGuid, IReadOnlyList<Appointment> appointmentsList)
        {
            //сохраню исходный ввод пользака
            string startInput = input;

            if (input.StartsWith("/del "))
                input = "/del";
            else
                input = "/updateappointment";
            

            if (appointmentsList.Count != 0)
            {
                if (!Guid.TryParse(startInput.Substring(input.Length), out taskGuid))
                {
                    throw new ArgumentException($"Введён некорректный номер записи.\n");
                }

                if (appointmentsList.FirstOrDefault(x => x.Id == taskGuid) == null)
                {
                    throw new ArgumentException($"Введён некорректный номер записи.\n");
                }
                return (input, taskGuid);
            }
            return (input, taskGuid);
        }









        /// <summary>
        /// Парсит строку в значение указанного enum, игнорируя регистр. 
        /// Возвращает defaultValue, если парсинг не удался или строка пустая.
        /// </summary>
        /// <typeparam name="T">Тип enum</typeparam>
        /// <param name="value">Строка для парсинга</param>
        /// <param name="defaultValue">Значение по умолчанию</param>
        /// <returns>Значение enum или defaultValue</returns>
        public static T GetEnumValueOrDefault<T>(string value, T defaultValue) where T : struct, Enum
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return Enum.TryParse<T>(value, true, out var result) ? result : defaultValue;
        }
    }
}
