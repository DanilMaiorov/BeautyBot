using BeautyBot.src.BeautyBot.Core.DataAcess.Context;
using BeautyBot.src.BeautyBot.Core.DataAcess.Mappers;
using BeautyBot.src.BeautyBot.Core.Interfaces;
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
    }
}
