using LinqToDB.Data;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.Sql
{
    public interface IDataContextFactory<TDataContext> where TDataContext : DataConnection
    {
        TDataContext CreateDataContext();
    }
}
