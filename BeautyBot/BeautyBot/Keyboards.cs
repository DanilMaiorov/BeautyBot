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
        public static readonly ReplyKeyboardMarkup replyKeyboard1 = new ReplyKeyboardMarkup(new[]
{
    // Первый ряд (3 кнопки в ряд)
    new[]
    {
        new KeyboardButton("Кнопка 1"),
        new KeyboardButton("Кнопка 2"),
        new KeyboardButton("Кнопка 3")
    },
    
    // Второй ряд (одна кнопка на всю ширину)
    new[]
    {
       new KeyboardButton("Большая кнопка")
    }
})
        {
            ResizeKeyboard = true,    // Автоматическое изменение размера
            OneTimeKeyboard = false   // Остается открытой после использования
        };
    }
}
