using BeautyBot.src.BeautyBot.Core.DataAcess.Context;
using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using LinqToDB;
using LinqToDB.Data;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Database
{
    public class DatabaseInitializer
    {
        private readonly IDataContextFactory<BeautyBotDataContext> _factory;
        public DatabaseInitializer(IDataContextFactory<BeautyBotDataContext> datacontextFactory)
        {
            _factory = datacontextFactory;
        }

        public void Initialize()
        {
            using var dbContext = _factory.CreateDataContext();

            try
            {
                dbContext.CreateTable<BeautyBotUserModel>();
                Console.WriteLine("Таблица Users создана.");
            }
            catch (Exception)
            {
                Console.WriteLine("Таблица Users уже существует.");
            }

            try
            {
                dbContext.CreateTable<ProcedureModel>();
                Console.WriteLine("Таблица Procedures создана.");
            }
            catch (Exception)
            {
                Console.WriteLine("Таблица Procedures уже существует.");
            }

            try
            {
                dbContext.CreateTable<AppointmentModel>();
                Console.WriteLine("Таблица Appointments создана.");
            }
            catch (Exception)
            {
                Console.WriteLine("Таблица Appointments уже существует.");
            }

            try
            {
                dbContext.CreateTable<SlotModel>();
                Console.WriteLine("Таблица Slots создана.");
            }
            catch (Exception) 
            {
                Console.WriteLine("Таблица Slots уже существует.");
            }
            
            try
            {
                dbContext.Execute("ALTER TABLE \"Appointments\" ADD FOREIGN KEY (\"UserId\") REFERENCES \"Users\"(\"Id\");");
                Console.WriteLine("Внешний ключ Appointments_UserId добавлен.");
            }
            catch (Exception) { }

            try
            {
                dbContext.Execute("ALTER TABLE \"Appointments\" ADD FOREIGN KEY (\"ProcedureId\") REFERENCES \"Procedures\"(\"Id\");");
                Console.WriteLine("Внешний ключ Appointments_ProcedureId добавлен.");
            }
            catch (Exception) { }

            try
            {
                dbContext.Execute("ALTER TABLE \"Slots\" ADD FOREIGN KEY (\"AppointmentId\") REFERENCES \"Appointments\"(\"Id\");");
                Console.WriteLine("Внешний ключ Slots_AppointmentId добавлен.");
            }
            catch (Exception) { }

            dbContext.Execute("CREATE INDEX IF NOT EXISTS idx_users_telegramId ON \"Users\"(\"TelegramId\");");
            dbContext.Execute("CREATE INDEX IF NOT EXISTS idx_appointments_id ON \"Appointments\"(\"Id\");");
            dbContext.Execute("CREATE INDEX IF NOT EXISTS idx_procedures_id ON \"Procedures\"(\"Id\");");
        }
    }
}