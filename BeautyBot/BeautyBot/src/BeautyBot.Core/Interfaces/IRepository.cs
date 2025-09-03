namespace BeautyBot.src.BeautyBot.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task Add(T entity, CancellationToken ct);
    }
}
