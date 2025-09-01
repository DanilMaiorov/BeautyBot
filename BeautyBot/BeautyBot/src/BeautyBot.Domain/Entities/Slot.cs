namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class Slot
    {
        public DateTime Date { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public Guid? AppointmentId { get; set; }
    }
}