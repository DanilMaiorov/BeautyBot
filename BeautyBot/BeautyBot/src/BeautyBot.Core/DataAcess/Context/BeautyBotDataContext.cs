using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using LinqToDB;
using LinqToDB.Data;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Context
{
    public class BeautyBotDataContext : DataConnection
    {
        public BeautyBotDataContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString) { }

        public ITable<BeautyBotUserModel> BeautyBotUsers => this.GetTable<BeautyBotUserModel>();
    }
}
