using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Application.Models
{
    public class AppointmentStepConfig
    {
        public string Message { get; init; } = String.Empty;
        public ReplyKeyboardMarkup Keyboard { get; init; } = new ReplyKeyboardMarkup();

    }
}
