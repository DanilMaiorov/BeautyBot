using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
