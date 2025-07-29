using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    // Репозиторий для слотов
    public interface ISlotRepository : IRepository<Appointment>
    {
        Task Add(Appointment appointment, CancellationToken ct);
        Task<Dictionary<TimeOnly, bool>> GetSlots(DateOnly date, CancellationToken ct);
    }
}
