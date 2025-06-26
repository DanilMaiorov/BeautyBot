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
                new KeyboardButton("Старт")
            })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };


        public static readonly ReplyKeyboardMarkup firstStep = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Посмотреть текущие записи"),
                new KeyboardButton("Записаться")
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
                new KeyboardButton("Назад")
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };


        public static readonly ReplyKeyboardMarkup thirdStep = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("френч"),
                new KeyboardButton("гель-лак"),
                new KeyboardButton("назад")
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
                new KeyboardButton("назад")
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };

        public static readonly ReplyKeyboardMarkup chooseTime = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("6 утра"),
                new KeyboardButton("14:88 дня"),
                new KeyboardButton("назад")
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };



        public static readonly ReplyKeyboardMarkup approveDate = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Изменить дату"),
                new KeyboardButton("Верно")
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };



        public static readonly ReplyKeyboardMarkup approveTime = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Изменить время"),
                new KeyboardButton("Верно")
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };
    }
}
