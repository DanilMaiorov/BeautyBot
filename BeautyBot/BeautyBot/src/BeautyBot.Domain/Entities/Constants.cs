using BeautyBot.src.BeautyBot.Application.Models;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public static class Constants
    {
        /// <summary>
        /// Нейминги кнопок
        /// </summary>
        public const string Start = "Старт";
        public const string Accept = "Верно";
        public const string Back = "Назад";
        public const string Cancel = "Отмена";

        public const string ToDoAppointment = "Записаться";

        public const string ChangeDate = "Выбрать другую дату";
        public const string ChangeTime = "Выбрать другое время";

        public const string Manicure = "Маникюр";
        public const string Pedicure = "Педикюр";

        public const string FrenchManicure = "Маникюр френч";
        public const string FrenchPedicure = "Педикюр френч";

        public const string GelPolishManicure = "Маникюр гель-лак";
        public const string GelPolishPedicure = "Педикюр гель-лак";

        public const string ClassicManicure = "Класссический маникюр";
        public const string ClassicPedicure = "Класссический педикюр";
        //public const string Back = "Назад";
        //public const string Back = "Назад";
        //public const string Back = "Назад";

        public static readonly List<AppointmentStepConfig> StepsConfigManicure = new()
        {
            new() {
                Message = "Что хотите сделать?",
                Keyboard = Keyboards.firstStep
            },
            new() {
                Message = "Куда записываемся?",
                Keyboard = Keyboards.secondStep
            },
            new() {
                Message = "Выберите маникюр",
                Keyboard = Keyboards.thirdManicureStep
            },
            new() {
                Message = "Выберите дату",
                Keyboard = Keyboards.chooseDate
            },
            new() {
                Message = "Выберите время",
                Keyboard = Keyboards.chooseTime
            },
            new() {
                Message = "Выберите время",
                Keyboard = Keyboards.chooseTime
            }
        };

        public static readonly List<AppointmentStepConfig> StepsConfigPedicure = new()
        {
            new() {
                Message = "Что хотите сделать?",
                Keyboard = Keyboards.firstStep
            },
            new() {
                Message = "Куда записываемся?",
                Keyboard = Keyboards.secondStep
            },
            new() {
                Message = "Выберите педикюр",
                Keyboard = Keyboards.thirdPedicureStep
            },
            new() {
                Message = "Выберите дату",
                Keyboard = Keyboards.chooseDate
            },
            new() {
                Message = "Выберите время",
                Keyboard = Keyboards.chooseTime
            },
            new() {
                Message = "Выберите время",
                Keyboard = Keyboards.chooseTime
            }
        };
    }
}

