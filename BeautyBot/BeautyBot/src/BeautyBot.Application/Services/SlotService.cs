using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class SlotService : ISlotService
    {
        private readonly ISlotRepository _slotRepository;
        public async Task<Dictionary<TimeOnly, bool>> GetSlots(DateOnly date, CancellationToken ct)
        {
            return await _slotRepository.GetSlots(date, ct);
        }
        public SlotService(ISlotRepository repository)
        {
            _slotRepository = repository;
        }
    }
}
