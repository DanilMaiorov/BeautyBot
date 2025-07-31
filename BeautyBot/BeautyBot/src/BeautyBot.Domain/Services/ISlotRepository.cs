using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    // Репозиторий для слотов
    public interface ISlotRepository : IRepository<Appointment>
    {
        Task Add(Appointment appointment, CancellationToken ct);
        Task<Dictionary<TimeOnly, bool>> GetCurrentDayAvailableTimeSlots(DateOnly date, CancellationToken ct);
        Task UpdateSlot(DateOnly date, TimeOnly time);

        Task<Dictionary <DateOnly, Dictionary<TimeOnly, bool>>> GetAllDaySlots();



        Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct);
        Task<List<DateOnly>> GetAvailableDaySlots(CancellationToken ct);

    }
}
