using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface ISlotService
    {
        IEnumerable<Slot> GenerateDailySlots(DateTime date, int intervalMinutes);
        Task GenerateYearlySlots(CancellationToken ct);
        Task<IEnumerable<Slot>> GetSlotsByDate(DateOnly date, CancellationToken ct);
        Task<List<DateOnly>> GetUnavailableSlotsByDate(CancellationToken ct);
        Task UpdateSlotFromAppointment(Appointment appointment, CancellationToken ct);
        Task ResetSlotFromAppointment(Appointment appointment, CancellationToken ct);
    }
}
