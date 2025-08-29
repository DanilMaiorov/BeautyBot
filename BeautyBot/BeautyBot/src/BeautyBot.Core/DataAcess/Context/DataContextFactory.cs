using BeautyBot.src.BeautyBot.Infrastructure.Repositories.Sql;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Context
{
    public class DataContextFactory : IDataContextFactory<BeautyBotDataContext>
    {
        private readonly string _connectionString;

        //string connectionString = "Server=localhost;Port=5432;Database=BeautyBot;Username=postgres;Password=dan1q!jobana;";
        public DataContextFactory(string connectionString)
        {
            _connectionString = connectionString;
        }
        public BeautyBotDataContext CreateDataContext()
        {
            return new BeautyBotDataContext(_connectionString);
        }
    }
}
