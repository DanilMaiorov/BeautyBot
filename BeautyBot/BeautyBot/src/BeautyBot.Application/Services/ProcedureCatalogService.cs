using BeautyBot.src.BeautyBot.Domain.Repositories;
using BeautyBot.src.BeautyBot.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class ProcedureCatalogService : IProcedureCatalogService
    {
        private readonly IProcedureDefinitionRepository _procedureDefinitionRepository;

        public ProcedureCatalogService(IProcedureDefinitionRepository procedureDefinitionRepository)
        {
            _procedureDefinitionRepository = procedureDefinitionRepository;
        }

        //public IReadOnlyList<IProcedure> GetAllProcedures() => _procedureDefinitionRepository.GetAll();
        //public IReadOnlyList<T> GetProceduresByType<T>() where T : IProcedure => _procedureDefinitionRepository.GetProceduresByType<T>();
        //public IProcedure? GetProcedureById(Guid id) => _procedureDefinitionRepository.GetById(id);
    }
}
