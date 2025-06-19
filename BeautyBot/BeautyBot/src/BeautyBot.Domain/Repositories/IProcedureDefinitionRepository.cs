using BeautyBot.src.BeautyBot.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Domain.Repositories
{
    // Репозиторий для процедур (всех типов)
    public interface IProcedureDefinitionRepository : IRepository<IProcedure> // Или IRepository<ProcedureBase>
    {
        // Возможно, методы для получения процедур по типу, если нужно
        //IReadOnlyList<T> GetProceduresByType<T>() where T : IProcedure;
    }
}
