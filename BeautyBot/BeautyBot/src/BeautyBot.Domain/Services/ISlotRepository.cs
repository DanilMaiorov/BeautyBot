using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    // Репозиторий для слотов
    public interface ISlotRepository : IRepository<Appointment>
    {
        Task Add(Appointment appointment, CancellationToken ct);
        Task<Dictionary<TimeOnly, Appointment>> GetCurrentDayAvailableTimeSlots(DateOnly date, CancellationToken ct);
        Task UpdateSlot(Appointment appointment, CancellationToken ct);

        Task<Dictionary <DateOnly, Dictionary<TimeOnly, Appointment>>> GetAllDaySlots();



        Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct);
        Task<List<DateOnly>> GetAvailableDaySlots(CancellationToken ct);

    }
}
