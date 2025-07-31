using BeautyBot.src.BeautyBot.Domain.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface ISlotService
    {
        //Task<Dictionary<TimeOnly, bool>> GetSlots(DateOnly date, CancellationToken ct);

        Task UpdateSlot(DateOnly date, TimeOnly time, CancellationToken ct);


        Task<Dictionary<TimeOnly, bool>> GetCurrentDayAvailableTimeSlots(DateOnly date, CancellationToken ct);

        Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct);

        Task<List<DateOnly>> GetAvailableDaySlots(CancellationToken ct);
    }
}
