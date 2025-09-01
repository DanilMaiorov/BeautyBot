using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using LinqToDB;
using LinqToDB.Data;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Context
{
    public class BeautyBotDataContext : DataConnection
    {
        public BeautyBotDataContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString) { }

        public ITable<BeautyBotUserModel> BeautyBotUsers => this.GetTable<BeautyBotUserModel>();
        public ITable<AppointmentModel> Appointments => this.GetTable<AppointmentModel>();
        public ITable<ProcedureModel> Procedures => this.GetTable<ProcedureModel>();
        public ITable<SlotModel> Slots => this.GetTable<SlotModel>();
    }
}
