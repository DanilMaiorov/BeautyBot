using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
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

        public static readonly ReplyKeyboardMarkup chooseTime = new ReplyKeyboardMarkup(
            new[]
            {
                GetSlots(),

                new KeyboardButton(Constants.Back)
            })
        {
            ResizeKeyboard = false,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };

        private static KeyboardButton GetSlots()
        {
            return new KeyboardButton("6 утра");
        }



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



        public static InlineKeyboardMarkup dates = new InlineKeyboardMarkup(

            //CalendarGenerator.GenerateCalendar(DateTime.Now)
        );




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
        }
}


//public InlineKeyboardMarkup CreateDynamicKeyboard(List<NailService> services)
//{
//    var buttons = new List<IEnumerable<InlineKeyboardButton>>();

//    // Группируем по 2 кнопки в ряд
//    for (int i = 0; i < services.Count; i += 2)
//    {
//        var row = services
//            .Skip(i)
//            .Take(2)
//            .Select(s => InlineKeyboardButton.WithCallbackData(s.Name, s.Id.ToString()))
//            .ToArray();

//        buttons.Add(row);
//    }

//    // Добавляем кнопку отмены
//    buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("Отмена", "cancel") });

//    return new InlineKeyboardMarkup(buttons);
//}


//public ReplyKeyboardMarkup GenerateCalendarKeyboard(DateTime date)
//{
//    var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
//    var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
//    var dayOfWeek = (int)firstDayOfMonth.DayOfWeek;

//    var buttons = new List<KeyboardButton[]>();

//    // Заголовок
//    buttons.Add(new[] { KeyboardButton.WithText($"Выберите день - {date:MMMM yyyy}") });

//    // Дни недели (опционально)
//    buttons.Add(Enum.GetValues(typeof(DayOfWeek))
//        .Cast<DayOfWeek>()
//        .Select(d => KeyboardButton.WithText(d.ToString().Substring(0, 2)))
//        .ToArray());

//    // Генерация дней
//    var dayButtons = new List<KeyboardButton>();
//    // Пустые кнопки для первого ряда
//    for (int i = 0; i < dayOfWeek; i++)
//    {
//        dayButtons.Add(KeyboardButton.WithText(" "));
//    }

//    for (int day = 1; day <= daysInMonth; day++)
//    {
//        dayButtons.Add(KeyboardButton.WithText(day.ToString()));

//        if ((day + dayOfWeek) % 7 == 0 || day == daysInMonth)
//        {
//            buttons.Add(dayButtons.ToArray());
//            dayButtons.Clear();
//        }
//    }

//    // Кнопки навигации
//    buttons.Add(new[]
//    {
//        KeyboardButton.WithText("<"),
//        KeyboardButton.WithText("Сегодня"),
//        KeyboardButton.WithText(">")
//    });

//    return new ReplyKeyboardMarkup(buttons) { ResizeKeyboard = true };
//}