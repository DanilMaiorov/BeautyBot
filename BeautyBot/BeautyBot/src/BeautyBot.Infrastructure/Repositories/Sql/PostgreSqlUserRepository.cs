using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Entities.Repositories;
using BeautyBot.src.BeautyBot.Core.DataAcess.Context;
using LinqToDB;
using BeautyBot.src.BeautyBot.Core.DataAcess.Mappers;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class PostgreSqlUserRepository : IUserRepository
    {
        private readonly IDataContextFactory<BeautyBotDataContext> _factory;
        public PostgreSqlUserRepository(IDataContextFactory<BeautyBotDataContext> factory)
        {
            _factory = factory;
        }

        public async Task<BeautyBotUser?> GetUser(Guid userId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var user = await dbContext.BeautyBotUsers.FirstOrDefaultAsync(x => x.UserId == userId);

            return BeautyBotUserModelMapper.MapFromModel(user);
        }

        public async Task<BeautyBotUser?> GetUserByTelegramUserId(long telegramUserId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var user = await dbContext.BeautyBotUsers.FirstOrDefaultAsync(x => x.TelegramUserId == telegramUserId);

            return BeautyBotUserModelMapper.MapFromModel(user);
        }

        public async Task Add(BeautyBotUser user, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            await dbContext.InsertAsync(BeautyBotUserModelMapper.MapToModel(user), token: ct);

        }
    }
}
