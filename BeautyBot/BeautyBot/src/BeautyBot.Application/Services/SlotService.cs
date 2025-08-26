using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class SlotService : ISlotService
    {
        private readonly ISlotRepository _slotRepository;
        //public async Task<Dictionary<TimeOnly, bool>> GetSlots(DateOnly date, CancellationToken ct)
        //{
        //    return await _slotRepository.GetCurrentDayTimeSlots(date, ct);
        //}

        public async Task<Dictionary<TimeOnly, Appointment>> GetCurrentDayAvailableTimeSlots(DateOnly date, CancellationToken ct)
        {
            //await GetUnavailableDaySlots(date, ct);
            return await _slotRepository.GetCurrentDayAvailableTimeSlots(date, ct);
        }


        public async Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct)
        {
            return await _slotRepository.GetUnavailableDaySlots(ct);
        }



        public async Task UpdateSlot(Appointment appointment, CancellationToken ct)
        {
            await _slotRepository.UpdateSlot(appointment, ct);
        }



        public SlotService(ISlotRepository repository)
        {
            _slotRepository = repository;
        }




        public async Task<List<DateOnly>> GetAvailableDaySlots(CancellationToken ct)
        {
            return await _slotRepository.GetAvailableDaySlots(ct);
        }


        private async Task GetUnavailableDays()
        {
            var days = await _slotRepository.GetAllDaySlots();
        }
    }   
}
