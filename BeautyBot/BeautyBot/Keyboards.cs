using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            string PrevMonthCallback = "prev_month_";
            string NextMonthCallback = "next_month_";
            string DaySelectedCallback = "day_selected_";

            var keyboardButtons = new List<List<InlineKeyboardButton>>();

            // Add day names row
            var dayNamesRow = new List<InlineKeyboardButton>();
            for (int i = 0; i < 7; i++)
            {
                dayNamesRow.Add(InlineKeyboardButton.WithCallbackData(
                    CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames[(i + (int)DayOfWeek.Monday) % 7], // Start from Monday
                    "day_name_no_action" // No action for day names
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
                currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));

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
                        isDayAvailable ? day.ToString() : "✖",
                        isDayAvailable ? $"{DaySelectedCallback}{currentDay:yyyy-MM-dd}" : "day_unavailable"
                    ));
                }
                else
                {
                    currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));
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
                    currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));

                keyboardButtons.Add(currentRow);
            }

            // Add navigation row
            var navigationRow = new List<InlineKeyboardButton>();

            // Previous month button (only if not the starting month)
            if (displayMonth.Year > minDate.Year || (displayMonth.Year == minDate.Year && displayMonth.Month > minDate.Month))
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                    "<",
                    $"{PrevMonthCallback}{displayMonth.AddMonths(-1):yyyy-MM-dd}"
                ));
            }
            else
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button")); // Placeholder for alignment
            }

            navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                displayMonth.ToString("MMMM yyyy", CultureInfo.CurrentCulture),
                "month_display_no_action"
            ));

            // Next month button (only if there are available days in the next month within the 60-day range)
            if (displayMonth.AddMonths(1) <= maxDate.AddDays(1).Date) // Check if next month potentially contains valid dates
            {
                // Check if any day in the next month falls within the maxDate range
                var nextMonthFirstDay = new DateTime(displayMonth.AddMonths(1).Year, displayMonth.AddMonths(1).Month, 1);
                if (nextMonthFirstDay <= maxDate)
                {
                    navigationRow.Add(InlineKeyboardButton.WithCallbackData(
                        ">",
                        $"{NextMonthCallback}{displayMonth.AddMonths(1):yyyy-MM-dd}"
                    ));
                }
                else
                {
                    navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button")); // Placeholder for alignment
                }
            }
            else
            {
                navigationRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_button")); // Placeholder for alignment
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

    }
}