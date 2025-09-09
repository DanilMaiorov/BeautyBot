using BeautyBot.src;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.TelegramBot.Dtos;
using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot
{
    /// <summary>
    /// Статический класс с клавиатурами телеграм
    /// </summary>
    public static class Keyboards
    {
        public static readonly ReplyKeyboardMarkup start = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton(Constants.Start)
            })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };

        public static readonly ReplyKeyboardMarkup firstStep = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Посмотреть текущие записи"),
                new KeyboardButton(Constants.ToDoAppointment)
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };

        public static readonly ReplyKeyboardMarkup secondStep = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Маникюр"),
                new KeyboardButton("Педикюр"),
                new KeyboardButton(Constants.Back)
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };

        public static readonly ReplyKeyboardMarkup thirdManicureStep = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Гель-лак"),
                new KeyboardButton("Френч"),
                new KeyboardButton("Классический"),
                new KeyboardButton(Constants.Back)
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };

        public static readonly ReplyKeyboardMarkup thirdPedicureStep = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Гель-лак"),
                new KeyboardButton("Классический"),
                new KeyboardButton(Constants.Back)
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };

        public static readonly ReplyKeyboardMarkup chooseDate = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("1 января"),
                new KeyboardButton("28 июня"),
                new KeyboardButton(Constants.Back)
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };

        public static readonly ReplyKeyboardMarkup approveDate = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton(Constants.ChangeDate),
                new KeyboardButton(Constants.Accept)
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };

        public static readonly ReplyKeyboardMarkup approveTime = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton(Constants.ChangeTime),
                new KeyboardButton(Constants.Accept)
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };

        public static readonly ReplyKeyboardMarkup cancelOrBack  = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton(Constants.Cancel),
                new KeyboardButton(Constants.Back)
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };


        /// <summary>
        /// Создает inline-клавиатуру календаря для выбора даты с учетом доступных и недоступных дней.
        /// </summary>
        /// <param name="unavailableDays">Список недоступных дат (DateOnly), которые будут отмечены как занятые</param>
        /// <returns>InlineKeyboardMarkup с календарем текущего месяца и навигацией</returns>
        /// <remarks>
        /// Генерирует календарь на текущий месяц с ограничением от текущей даты до 60 дней вперед.
        /// Недоступные дни отмечаются символом "✖" и имеют callback "day_unavailable".
        /// Доступные дни отображаются числом и имеют callback в формате "day_selected_yyyy-MM-dd".
        /// Включает строку с названиями дней недели (начиная с понедельника) и навигационные кнопки для переключения месяцев.
        /// </remarks>
        public static InlineKeyboardMarkup DaySlotsKeyboard(DateTime newDisplayMonth, List<DateOnly> unavailableDays)
        {
            DateTime minDate = DateTime.Today;
            DateTime maxDate = DateTime.Today.AddDays(60);

            return DaySlotsKeyboard(newDisplayMonth, minDate, maxDate, unavailableDays);
        }

        /// <summary>
        /// Создает inline-клавиатуру календаря для выбора даты с пользовательскими параметрами отображения.
        /// </summary>
        /// <param name="displayMonth">Месяц и год, которые должны отображаться в календаре</param>
        /// <param name="minDate">Минимальная доступная дата для выбора</param>
        /// <param name="maxDate">Максимальная доступная дата для выбора</param>
        /// <param name="unavailableDays">Список недоступных дат (DateOnly), которые будут отмечены как занятые</param>
        /// <returns>InlineKeyboardMarkup с календарем указанного месяца и навигацией</returns>
        /// <remarks>
        /// Генерирует календарь для указанного месяца с пользовательскими границами выбора дат.
        /// Недоступные дни отмечаются символом "✖" и имеют callback "day_unavailable".
        /// Доступные дни отображаются числом и имеют callback в формате "day_selected_yyyy-MM-dd".
        /// Включает строку с названиями дней недели (начиная с понедельника) и навигационные кнопки для переключения месяцев.
        /// Навигационные кнопки отображаются только если соответствующий месяц попадает в диапазон допустимых дат.
        /// </remarks>
        public static InlineKeyboardMarkup DaySlotsKeyboard(
            DateTime displayMonth,
            DateTime minDate,
            DateTime maxDate,
            List<DateOnly> unavailableDays)
        {
            string PrevMonthCallback = "prev_month";
            string NextMonthCallback = "next_month";
            string DaySelectedCallback = "day_selected";

            var keyboardButtons = new List<List<InlineKeyboardButton>>();

            // Add day names row
            var dayNamesRow = new List<InlineKeyboardButton>();
            for (int i = 0; i < 7; i++)
            {
                dayNamesRow.Add(InlineKeyboardButton.WithCallbackData(
                    text: CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(i + (int)DayOfWeek.Monday) % 7], // Start from Monday
                    callbackData: new CalendarDayCallbackDto { Action = "day_name_no_action", Date = default}.ToString() // No action for day names
                ));
            }
            keyboardButtons.Add(dayNamesRow);

            // Add month days
            var firstDayOfMonth = new DateTime(displayMonth.Year, displayMonth.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(displayMonth.Year, displayMonth.Month);

            // Calculate offset for the first day of the month (0 for Monday, 6 for Sunday)
            int offset = ((int)firstDayOfMonth.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;

            var currentRow = new List<InlineKeyboardButton>();

            // Add empty buttons for the days before the first day of the month
            for (int i = 0; i < offset; i++)
                currentRow.Add(InlineKeyboardButton.WithCallbackData(
                    text: " ",
                    callbackData: new CalendarDayCallbackDto { Action = "empty_day", Date = default }.ToString()
                ));
            

            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDay = new DateTime(displayMonth.Year, displayMonth.Month, day);
                var currentDateOnly = DateOnly.FromDateTime(currentDay);
                // Check if the day is within the allowed range
                bool isDayValid = currentDay >= minDate && currentDay <= maxDate;
                bool isDayAvailable = isDayValid && !unavailableDays.Contains(currentDateOnly);

                if (isDayValid)
                {
                    currentRow.Add(InlineKeyboardButton.WithCallbackData(
                        text: isDayAvailable ? day.ToString() : "✖",
                        callbackData: (isDayAvailable
                            ? new CalendarDayCallbackDto { Action = DaySelectedCallback, Date = DateOnly.FromDateTime(currentDay.Date)}
                            : new CalendarDayCallbackDto { Action = "day_unavailable", Date = default })
                            .ToString()
                    ));    
                }
                else
                {
                    currentRow.Add(InlineKeyboardButton.WithCallbackData(
                        text: " ",
                        callbackData: new CalendarDayCallbackDto { Action = "empty_day", Date = default }.ToString()
                     ));
                }

                if (currentRow.Count == 7)
                {
                    keyboardButtons.Add(currentRow);
                    currentRow = new List<InlineKeyboardButton>();
                }
            }
            // Add remaining empty buttons for the last row
            if (currentRow.Any())
            {
                while (currentRow.Count < 7)
                    currentRow.Add(InlineKeyboardButton.WithCallbackData(
                        text: " ",
                        callbackData: new CalendarDayCallbackDto { Action = "empty_day", Date = default }.ToString()
                    ));

                keyboardButtons.Add(currentRow);
            }

            // Add navigation row
            var navigationRow = new List<InlineKeyboardButton>();

            // Previous month button (only if not the starting month)
            if (displayMonth.Year > minDate.Year || (displayMonth.Year == minDate.Year && displayMonth.Month > minDate.Month))
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                    text: "<",
                    callbackData: new CalendarMonthCallbackDto
                    {
                        Action = "prev_month",
                        Month = displayMonth.AddMonths(-1).ToString("yyyy-MM-dd")
                    }.ToString()
                ));
            }
            else
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button")); // Placeholder for alignment
            }

            navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                text: displayMonth.ToString("MMMM yyyy", CultureInfo.CurrentCulture),
                callbackData: new CalendarMonthCallbackDto
                {
                    Action = "month_display_no_action",
                    Month = displayMonth.ToString("yyyy-MM-dd")
                }.ToString()
            ));

            // Next month button (only if there are available days in the next month within the 60-day range)
            if (displayMonth.AddMonths(1) <= maxDate.AddDays(1).Date) // Check if next month potentially contains valid dates
            {
                // Check if any day in the next month falls within the maxDate range
                var nextMonthFirstDay = new DateTime(displayMonth.AddMonths(1).Year, displayMonth.AddMonths(1).Month, 1);
                if (nextMonthFirstDay <= maxDate)
                {
                    navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                        text: ">",
                        callbackData: new CalendarMonthCallbackDto
                        {
                            Action = "next_month",
                            Month = displayMonth.AddMonths(1).ToString("yyyy-MM-dd")
                        }.ToString()
                    ));
                }
                else
                {
                    navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                        text: " ",
                        callbackData: new CalendarMonthCallbackDto { Action = "empty_button", Month = null }.ToString()
                    ));
                }
            }
            else
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                    text: " ",
                    callbackData: new CalendarMonthCallbackDto { Action = "empty_button", Month = null }.ToString()
                ));
            }

            keyboardButtons.Add(navigationRow);

            return new InlineKeyboardMarkup(keyboardButtons);
        }

        /// <summary>
        /// Создает клавиатуру с кнопками временных слотов для выбора пользователем.
        /// </summary>
        /// <param name="slots">Коллекция объектов Slot, представляющих доступные временные интервалы</param>
        /// <returns>Объект ReplyKeyboardMarkup с кнопками временных слотов и кнопками отмены/назад</returns>
        /// <remarks>
        /// В нижней части клавиатуры добавляются стандартные кнопки отмены и возврата назад.
        /// </remarks>
        public static ReplyKeyboardMarkup TimeSlotsKeyboard(IEnumerable<Slot> slots)
        {
            if (slots.Count() == 0)
                Console.WriteLine("На выбранную дату записей нет");

            var timeSlotButtons = slots
                .Select(button => new KeyboardButton(TimeOnly.FromDateTime(button.StartTime).ToString()))
                .Select(btn => new[] { btn })
                .Concat(cancelOrBack.Keyboard)
                .ToArray();

            return new ReplyKeyboardMarkup(timeSlotButtons)
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };
        }

        public static InlineKeyboardMarkup AppointmentItemKeyboard(Appointment appointment)
        {
            var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>();

            //последний ряд кнопок
            keyboardRows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    text: "❌ Отменить", 
                    callbackData: new AppointmentCallbackDto { Action = "cancel_appointment", AppointmentId = appointment.Id }.ToString()),
                InlineKeyboardButton.WithCallbackData(
                    text: "➡️ Перенести", 
                    callbackData: new AppointmentCallbackDto { Action = "edit_appointment", AppointmentId = appointment.Id }.ToString())
            });
            keyboardRows.Add(new[]
{
                InlineKeyboardButton.WithCallbackData(
                    text: "↩️ Назад к списку",
                    callbackData: new PagedListCallbackDto { Action = "list_appointments", Page = 0 }.ToString())
            });

            return new InlineKeyboardMarkup(keyboardRows);
        }

        public static InlineKeyboardMarkup AppointmentEditItemKeyboard(Appointment appointment)
        {
            var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>();

            //последний ряд кнопок
            keyboardRows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    text: "Перенести дату и время",
                    callbackData: new AppointmentCallbackDto { Action = "change_date", AppointmentId = appointment.Id }.ToString()),
                InlineKeyboardButton.WithCallbackData(
                    text: "Перенести время",
                    callbackData: new AppointmentCallbackDto { Action = "change_time", AppointmentId = appointment.Id }.ToString())
            });
            keyboardRows.Add(new[]
{
                InlineKeyboardButton.WithCallbackData(
                    text: "↩️ Назад к списку",
                    callbackData: new PagedListCallbackDto { Action = "list_appointments", Page = 0 }.ToString())
            });

            return new InlineKeyboardMarkup(keyboardRows);
        }


        /// <summary>
        /// Генерирует кнопки с записями
        /// </summary>
        /// <param name="appointments">Коллекция записей</param>
        /// <param name="keyboardRows">Список с кнопками</param>
        /// <param name="action">Действие</param>
        private static void ListInlineButtonGenerate(IReadOnlyList<Appointment> appointments, List<IEnumerable<InlineKeyboardButton>> keyboardRows, string action)
        {
            keyboardRows.AddRange(appointments.Select(appointment =>
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        text: Helper.FormatAppointmentString(appointment.Procedure, appointment.AppointmentDate),
                        callbackData: new AppointmentCallbackDto { Action = action, AppointmentId = appointment.Id }.ToString()
                    )
                }
            ));
        }


        public static void GetAppointmentListKeyboardWithPagination(
            IEnumerable<KeyValuePair<string, string>> items,
            List<IEnumerable<InlineKeyboardButton>> keyboardRows,
            PagedListCallbackDto listDto,
            int totalPages)
        {
            keyboardRows.AddRange(items.Select(item =>
            {
                Guid.TryParse(item.Key, out var id);
                return new[]
                {
                InlineKeyboardButton.WithCallbackData(
                    text: item.Value,
                    callbackData: new AppointmentCallbackDto { Action = "show_ap", AppointmentId = id }.ToString()
                )
            };
            }));

            //логика кнопок пагинации
            var paginationButtons = new List<InlineKeyboardButton>();

            //логика кнопки "Назад"
            if (listDto.Page > 0)
            {
                paginationButtons.Add(
                    InlineKeyboardButton.WithCallbackData(
                        text: "⬅️",
                        // Правильно ссылаемся на предыдущую страницу.
                        callbackData: new PagedListCallbackDto { Action = "list_appointments", Page = listDto.Page - 1 }.ToString()
                    )
                );
            }

            //логика номера текущей страницы для контекста
            paginationButtons.Add(
                InlineKeyboardButton.WithCallbackData(
                    text: $"{listDto.Page + 1}/{totalPages}",
                    callbackData: "no_action" // Кнопка, которая не делает ничего при нажатии.
                )
            );

            //логика кнопки "Вперёд"
            if (listDto.Page < totalPages - 1)
            {
                paginationButtons.Add(
                    InlineKeyboardButton.WithCallbackData(
                        text: "➡️",
                        // Правильно ссылаемся на следующую страницу.
                        callbackData: new PagedListCallbackDto { Action = "list_appointments", Page = listDto.Page + 1 }.ToString()
                    )
                );
            }

            //логика кнопок пагинации, добавляем их в новый ряд
            if (paginationButtons.Any())
                keyboardRows.Add(paginationButtons.ToArray());
        }


        //метод клавиатуры подтверждения удаления списка
        public static InlineKeyboardMarkup GetApproveCancelAppointmentKeyboard()
        {
            //первый ряд
            var keyboardRows = new List<IEnumerable<InlineKeyboardButton>>();

            //последний ряд кнопок
            keyboardRows.Add(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: "✅ Да", callbackData: "approve_cancel"),
                InlineKeyboardButton.WithCallbackData(text: "❌ Нет", callbackData: "decline_cancel")
            });

            return new InlineKeyboardMarkup(keyboardRows);
        }
    }
}