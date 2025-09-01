namespace BeautyBot.src.BeautyBot.Core.DataAcess.Context
{
    public class DataContextFactory : IDataContextFactory<BeautyBotDataContext>
    {
        private readonly string _connectionString;
        private readonly string _providerName;

        public DataContextFactory(string connectionString, string providerName)
        {
            _connectionString = connectionString;
            _providerName = providerName;
        }
        public BeautyBotDataContext CreateDataContext()
        {
            return new BeautyBotDataContext(_connectionString, _providerName);
        }
    }
}
