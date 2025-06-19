using BeautyBot.src.BeautyBot.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class InMemoryProcedureDefinitionRepository : IProcedureDefinitionRepository
    {
        //private readonly List<IProcedure> _procedures = new List<IProcedure>(); // Или List<ProcedureBase>
        //public InMemoryProcedureDefinitionRepository()
        //{
        //    // Заполнение начальными данными
        //    _procedures.Add(new GelPolishManicure());
        //    _procedures.Add(new FrenchManicure());
        //    _procedures.Add(new Brows("Коррекция бровей", 800));
        //    _procedures.Add(new Lashes("Наращивание ресниц 2D", 2500));
        //}
        //public IReadOnlyList<T> GetProceduresByType<T>() where T : IProcedure => _procedures.OfType<T>().ToList().AsReadOnly();
        //public IProcedure? GetById(Guid id) => _procedures.FirstOrDefault(p => p.Id == id);
        //public IReadOnlyList<IProcedure> GetAll() => _procedures.AsReadOnly();
        //public void Add(IProcedure entity) { _procedures.Add(entity); }
        //public void Update(IProcedure entity) { /* ... */ }
        //public void Delete(Guid id) { _procedures.RemoveAll(p => p.Id == id); }
    }
}
