using System.Globalization;

namespace BeautyBot.src.BeautyBot.TelegramBot.Dtos
{
    public class CalendarMonthCallbackDto : CallbackDto
    {
        public string Month { get; set; }

        public static new CalendarMonthCallbackDto FromString(string input)
        {
            var parts = input.Split('|');
            return new CalendarMonthCallbackDto
            {
                Action = parts[0],
                Month = GetFormattedMonth(parts)
            };
        }

        public override string ToString()
        {
            return $"{base.ToString()}|{Month}";
        }

        private static string GetFormattedMonth(string[] parts)
        {
            if (parts.Length == 1)
                return null;

            if (!parts[0].StartsWith("prev_month", StringComparison.OrdinalIgnoreCase) &&
                !parts[0].StartsWith("next_month", StringComparison.OrdinalIgnoreCase)
                )
            {
                return "Неверный формат данных";
            }

            if (DateTime.TryParseExact(parts[parts.Length - 1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                return date.ToString("MM", CultureInfo.CurrentCulture);
            else
                return null;
        }
    }
}
