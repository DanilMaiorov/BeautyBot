using BeautyBot.src.BeautyBot.Core.DataAcess.Context;
using BeautyBot.src.BeautyBot.Core.DataAcess.Mappers;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using LinqToDB;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class PostgreSqlProcedureRepository
    {

        private readonly IDataContextFactory<BeautyBotDataContext> _factory;

        public PostgreSqlProcedureRepository(IDataContextFactory<BeautyBotDataContext> factory)
        {
            _factory = factory;
        }

        public async Task Add(IProcedure procedure, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            await dbContext.InsertAsync(ProcedureModelMapper.MapToModel(procedure));
        }

        //public async Task<IEnumerable<IProcedure>> GetAllByType(string type, CancellationToken ct)
        //{
        //    using var dbContext = _factory.CreateDataContext();

        //    var procedures = await dbContext.Procedures
        //        .Where(p => p.Type == type)
        //        .ToListAsync();
        //}

        //public async Task<IEnumerable<IProcedure>> GetAllBySubtype(string subtype, CancellationToken ct)
        //{
        //    using var dbContext = _factory.CreateDataContext();

        //    var procedures = await dbContext.Procedures
        //        .Where(p => p.Subtype == subtype)
        //        .ToListAsync();
        //}

    }
}
