namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.Sql
{
    public class DataContextFactory : IDataContextFactory<ToDoDataContext>
    {
        private string connectionString = "Server=localhost;Port=5432;Database=ToDoList;Username=postgres;Password=dan1q!jobana;";
        public ToDoDataContext CreateDataContext()
        {
            return new ToDoDataContext(connectionString);
        }
    }
}
