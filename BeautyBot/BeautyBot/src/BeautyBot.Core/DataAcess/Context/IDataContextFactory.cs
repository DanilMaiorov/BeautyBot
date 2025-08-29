using LinqToDB.Data;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Context
{
    public interface IDataContextFactory<TDataContext> where TDataContext : DataConnection
    {
        TDataContext CreateDataContext();
    }
}
