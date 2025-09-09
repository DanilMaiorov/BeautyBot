namespace BeautyBot.src.BeautyBot.Domain.Entities.Repositories
{
    // Репозиторий для слотов
    public interface ISlotRepository //: IRepository<Slot>
    {
        Task AddRange(IEnumerable<Slot> entities, CancellationToken ct);
        Task<IEnumerable<Slot>> GetSlotsByDate(DateOnly date);
        Task<bool> AnySlotsExist();
        Task UpdateSlot(DateOnly date, TimeOnly time, Guid appointmentId, CancellationToken ct);
        Task ResetSlot(DateOnly date, TimeOnly time, Guid appointmentId, CancellationToken ct);
        Task<List<DateOnly>> GetUnavailableDaySlots(CancellationToken ct);
    }
}
