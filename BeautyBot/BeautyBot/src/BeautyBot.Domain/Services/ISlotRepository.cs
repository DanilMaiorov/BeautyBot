using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    // Репозиторий для слотов
    public interface ISlotRepository //: IRepository<Slot>
    {
        Task AddRange(IEnumerable<Slot> entities, CancellationToken ct);
        Task<IEnumerable<Slot>> GetSlotsByDate(DateOnly date);
        Task<bool> AnySlotsExist();
        Task UpdateSlot(DateOnly date, TimeOnly time, Guid appointmentId, CancellationToken ct);
        Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct);


        //Task<Dictionary<TimeOnly, Appointment>> GetCurrentDayAvailableTimeSlots(DateOnly date, CancellationToken ct);

        //Task<Dictionary <DateOnly, Dictionary<TimeOnly, Appointment>>> GetAllDaySlots();
        //Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct);
        //Task<List<DateOnly>> GetAvailableDaySlots(CancellationToken ct);
    }
}
