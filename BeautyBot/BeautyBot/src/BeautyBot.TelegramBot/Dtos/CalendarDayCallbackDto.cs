using System.Globalization;

namespace BeautyBot.src.BeautyBot.TelegramBot.Dtos
{
    public class CalendarDayCallbackDto : CallbackDto
    {
        public DateOnly Date { get; set; }

        public static new CalendarDayCallbackDto FromString(string input)
        {
            var parts = input.Split('|');
            return new CalendarDayCallbackDto
            {
                Action = parts[0],
                Date = ParseDateFromString(parts),
            };
        }

        public override string ToString()
        {
            return $"{base.ToString()}|{Date}";
        }

        private static DateOnly ParseDateFromString(string[] parts)
        {
            if (parts.Length == 1)
                return default;

            Console.WriteLine(parts[parts.Length - 1]);

            if (DateOnly.TryParseExact(parts[parts.Length - 1], "dd.MM.yyyy",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly result))
            {
                return result;
            }

            return default;
        }
    }
}
