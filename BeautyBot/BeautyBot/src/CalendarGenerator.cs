using System.Globalization;
using Telegram.Bot.Types.ReplyMarkups;

public static class CalendarGenerator
{
    private const string PrevMonthCallback = "prev_month_";
    private const string NextMonthCallback = "next_month_";
    private const string DaySelectedCallback = "day_selected_";

    public static InlineKeyboardMarkup GenerateCalendar(DateTime currentDate)
    {
        return GenerateCalendarInternal(currentDate, DateTime.Today, DateTime.Today.AddDays(60));
    }

    private static InlineKeyboardMarkup GenerateCalendarInternal(DateTime displayMonth, DateTime minDate, DateTime maxDate)
    {
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
        {
            currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));
        }

        for (int day = 1; day <= daysInMonth; day++)
        {
            var currentDay = new DateTime(displayMonth.Year, displayMonth.Month, day);

            // Check if the day is within the allowed range
            bool isDayValid = currentDay >= minDate && currentDay <= maxDate;

            if (isDayValid)
            {
                currentRow.Add(InlineKeyboardButton.WithCallbackData(
                    day.ToString(),
                    $"{DaySelectedCallback}{currentDay:yyyy-MM-dd}"
                ));
            }
            else
            {
                // Disable days outside the 60-day window or past days
                currentRow.Add(InlineKeyboardButton.WithCallbackData(
                    day.ToString(),
                    $"{day} {displayMonth.ToString("dd MM", CultureInfo.CurrentCulture)}"
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
            {
                currentRow.Add(InlineKeyboardButton.WithCallbackData(" ", "empty_day"));
            }
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
}