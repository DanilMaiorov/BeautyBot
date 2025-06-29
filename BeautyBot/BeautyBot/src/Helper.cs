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
        public static async (string, string, string, string, string, Guid) InputCheck(string input, IReadOnlyList<Appointment> currentUserAppointmentsList = null)
        {
            string cutInput = "";
            Guid taskGuid = Guid.Empty;

            string month = "";
            string date = "";
            string time = "";

            if (input.StartsWith("/add") || input.StartsWith("/del") || input.StartsWith("/updateappointment") || input.StartsWith("/find"))
            {
                if (input.StartsWith("/add "))
                {
                    cutInput = input.Substring(5);
                    input = "/add";
                }
                else if (input.StartsWith("/del ") || input.StartsWith("/updateappointment "))
                {
                    //верну данные кортежем
                    (string command, Guid taskGuid) inputData = Validate(input, taskGuid, currentUserAppointmentsList);

                    input = inputData.command;
                    taskGuid = inputData.taskGuid;
                }
                else
                {
                    input = "unregistered user command";
                }
            }




            if (input.StartsWith("day_selected_"))
            {
                date = GetFormattedMonthDateTime(input);
                input = "/date";
            }

            if (input.StartsWith("time_selected_"))
            {
                time = GetFormattedMonthDateTime(input);
                input = "/time";
            }

            if (input.StartsWith("prev_month_", StringComparison.OrdinalIgnoreCase) || input.StartsWith("next_month_", StringComparison.OrdinalIgnoreCase))
            {
                month = GetFormattedMonthDateTime(input);
                input = "/changemonth";

            }
            //// Обработка других кнопок (дни недели, пустые дни, отключенные дни, отображение месяца)
            //else
            //{
            //    // Просто закрываем всплывающее уведомление, так как эти кнопки не требуют других действий
            //    await _botClient.AnswerCallbackQueryAsync(callbackQuery.Id);
            //}
            



            switch (input)
            {
                case "Старт":
                    input = "/start";
                    break;

                case "Записаться":
                    input = "/create";
                    break;




                case "Маникюр":
                    input = "/manicure";
                    break;

                case "Педикюр":
                    input = "/pedicure";
                    break;




                case "Френч":
                    input = "/french";
                    break;

                case "Гель-лак":
                    input = "/gelPolish";
                    break;

                case "Классический":
                    input = "/classic";
                    break;




                case "1 января":
                    date = input;
                    input = "/date";
                    break;



                case "6 утра":
                    time = input;
                    input = "/time";
                    break;


                case "Верно":
                    input = "/approve";
                    break;

                case "Назад":
                    input = "/back";
                    break;


                default:
                    break;
            }
            await Task.Delay(1);
            return (input, cutInput, month, date, time, taskGuid);
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


        public static string GetFormattedMonthDateTime(string callbackData)
        {
            if (!callbackData.StartsWith("day_selected_", StringComparison.OrdinalIgnoreCase) ||
                !callbackData.StartsWith("time_selected_", StringComparison.OrdinalIgnoreCase) ||
                !callbackData.StartsWith("prev_month_", StringComparison.OrdinalIgnoreCase) ||
                !callbackData.StartsWith("next_month_", StringComparison.OrdinalIgnoreCase) 
                )
            {
                return "Неверный формат данных";
            }

            string dateString = callbackData.Replace("day_selected_", "");
            string timeString = callbackData.Replace("time_selected_", "");
            string prevMonthString = callbackData.Replace("prev_month_", "");
            string nextMonthString = callbackData.Replace("next_month_", "");


            // Пытаемся распарсить строку даты
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
            {
                return date.ToString("dd MMMM", CultureInfo.CurrentCulture);
            }
            else
            {
                return "Ошибка парсинга даты";
            }
        }



        // метод валидации задачи
        public static (string, Guid) Validate(string input, Guid taskGuid, IReadOnlyList<Appointment> appointmentsList)
        {
            //сохраню исходный ввод пользака
            string startInput = input;

            if (input.StartsWith("/del "))
            {
                input = "/del";
            }
            else
            {
                input = "/updateappointment";
            }

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
    }
}
