namespace BeautyBot.src.BeautyBot.Core.DataAcess.Context
{
    public class DataContextFactory : IDataContextFactory<BeautyBotDataContext>
    {
        private readonly string _connectionString;

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
