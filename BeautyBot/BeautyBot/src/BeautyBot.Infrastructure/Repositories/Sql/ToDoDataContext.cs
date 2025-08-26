using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using LinqToDB;
using LinqToDB.Data;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.Sql
{
    public class ToDoDataContext : DataConnection
    {
        public ToDoDataContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString) { }

        public ITable<BeautyBotUserModel> BeautyBotUsers => this.GetTable<BeautyBotUserModel>();
        public ITable<AppointmentModel> Appointments => this.GetTable<AppointmentModel>();
    }
}
