using BeautyBot.src.BeautyBot.Domain.Entities;
using Otus.ToDoList.ConsoleBot.Types;
using Otus.ToDoList.ConsoleBot;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautyBot.src.BeautyBot.Core.Enums;
using System.Collections;

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

            await botClient.SendMessage(chat, builder.ToString(), ct);

            //рендерю менюшку
            //await botClient.SetMyCommands(commands, cancellationToken: ct);
        }

        /// <summary>
        /// Метод форматирования ввода команды от пользователя /add, /removetask, /completetask, /find
        /// </summary>
        /// <param name="input">Ввод пользователя</param>
        /// <param name="currentUserAppointmentsList">Список записей юзера</param>
        /// <returns>Кортеж с данными по записи</returns>
        public static (string, string, Guid) InputCheck(string input, IReadOnlyList<Appointment> currentUserAppointmentsList = null)
        {
            string cutInput = "";
            Guid taskGuid = Guid.Empty;

            if (input.StartsWith("/add") || input.StartsWith("/removetask") || input.StartsWith("/updateappointment") || input.StartsWith("/find"))
            {
                if (input.StartsWith("/add "))
                {
                    cutInput = input.Substring(5);
                    input = "/add";
                }
                //else if (input.StartsWith("/find "))
                //{
                //    cutInput = input.Substring(6);
                //    input = "/find";
                //}
                else if (input.StartsWith("/removetask ") || input.StartsWith("/updateappointment "))
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
           
            return (input, cutInput, taskGuid);
        }



        /// <summary>
        /// Метод рендера списка записей на процедуры
        /// </summary>
        /// <param name="appointments"></param>
        /// <param name="botClient"></param>
        /// <param name="chat"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async static Task AppointmentsListRender(IReadOnlyList<Appointment> appointments, ITelegramBotClient botClient, Chat chat, CancellationToken ct)
        {
            int appointmentCounter = 0;

            var builder = new StringBuilder();

            foreach (Appointment appointment in appointments)
            {
                appointmentCounter++;
                await botClient.SendMessage(chat, $"{appointmentCounter}) ({appointment.State}) {appointment.Procedure.Name} - {appointment.CreatedAt}", ct);

                //тут надо попробовать выводить запись вместе с кнопками разными сообщениями
                //await botClient.SendMessage(chat, $"\n{appointment.Id}", ct);
            }
        }



        // метод валидации задачи
        public static (string, Guid) Validate(string input, Guid taskGuid, IReadOnlyList<Appointment> appointmentsList)
        {
            //сохраню исходный ввод пользака
            string startInput = input;

            if (input.StartsWith("/removetask "))
            {
                input = "/removetask";
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
