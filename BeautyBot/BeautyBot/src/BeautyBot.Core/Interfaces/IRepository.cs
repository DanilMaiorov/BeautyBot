namespace BeautyBot.src.BeautyBot.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        //    T? GetById(Guid id);
        //    IReadOnlyList<T> GetAll();
        Task Add(T entity, CancellationToken ct);
        //    void Update(T entity);
        //    void Delete(Guid id);
    }
}
