namespace BeautyBot.src.BeautyBot.TelegramBot.Dtos
{
    public class AppointmentCallbackDto : CallbackDto
    {
        public Guid? AppointmentId { get; set; }

        public static new AppointmentCallbackDto FromString(string input)
        {
            var parts = input.Split('|');
            return new AppointmentCallbackDto
            {
                Action = parts[0],
                AppointmentId = parts.Length > 1 && Guid.TryParse(parts[1], out var id) ? id : null
            };
        }

        public override string ToString()
        {
            return $"{base.ToString()}|{AppointmentId}";
        }
    }
}
