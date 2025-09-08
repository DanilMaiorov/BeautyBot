namespace BeautyBot.src.BeautyBot.TelegramBot.Dtos
{
    public class PagedListCallbackDto : AppointmentCallbackDto
    {
        public int Page { get; set; }

        public static new PagedListCallbackDto FromString(string input)
        {
            var parts = input.Split('|');

            var baseDto = AppointmentCallbackDto.FromString(input);

            return new PagedListCallbackDto
            {
                Action = baseDto.Action,
                AppointmentId = baseDto.AppointmentId,
                Page = parts.Length > 2 && int.TryParse(parts[2], out var page) ? page : 0
            };
        }

        public override string ToString()
        {
            return $"{base.ToString()}|{Page}";
        }
    }
}
