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
        public static readonly ReplyKeyboardMarkup keyboardStart = new ReplyKeyboardMarkup(
            new[]
            {
                new KeyboardButton("Старт")
            })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = true
        };
        public static readonly ReplyKeyboardMarkup keyboardFirstStep = new ReplyKeyboardMarkup(
            // Первый ряд (3 кнопки в ряд)
            new[]
            {
                new KeyboardButton("а тут чё?"),
                new KeyboardButton("Посмотреть текущие записи"),
                new KeyboardButton("Записаться на процедуру")
            })
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };
    }
}
