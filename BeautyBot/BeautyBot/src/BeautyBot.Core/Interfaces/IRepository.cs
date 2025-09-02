namespace BeautyBot.src.BeautyBot.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        //    T? GetById(Guid id);
        //    IReadOnlyList<T> GetAll();
        Task Add(T entity, CancellationToken ct);
        //Task Update(T entity, CancellationToken ct);
        //    void Delete(Guid id);
    }
}
