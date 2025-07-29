using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface ISlotService
    {
        Task<Dictionary<TimeOnly, bool>> GetSlots(DateOnly date, CancellationToken ct);
    }
}
