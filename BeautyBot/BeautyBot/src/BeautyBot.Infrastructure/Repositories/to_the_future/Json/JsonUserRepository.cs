//using BeautyBot.src.BeautyBot.Domain.Repositories;

//namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.to_the_future.Json
//{

//    //Аналогично для JsonProcedureDefinitionRepository и JsonAppointmentRepository.Важно будет учесть 
//    //сериализацию/десериализацию интерфейсов(IProcedure) и полиморфных типов(различных производных процедур) 
//    //с использованием JsonSerializer(например, через JsonDerivedType атрибуты или кастомные конвертеры).
//    public class JsonUserRepository : IUserRepository
//    {
//        //private readonly string _filePath = "users.json";
//        //private List<ToDoUser> _users;

//        //public JsonUserRepository()
//        //{
//        //    LoadData();
//        //}

//        //private void LoadData()
//        //{
//        //    if (File.Exists(_filePath))
//        //    {
//        //        var json = File.ReadAllText(_filePath);
//        //        _users = JsonSerializer.Deserialize<List<ToDoUser>>(json) ?? new List<ToDoUser>();
//        //    }
//        //    else
//        //    {
//        //        _users = new List<ToDoUser>();
//        //    }
//        //}

//        //private void SaveData()
//        //{
//        //    var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
//        //    File.WriteAllText(_filePath, json);
//        //}

//        //public ToDoUser? GetByTelegramId(long telegramUserId) => _users.FirstOrDefault(u => u.TelegramUserId == telegramUserId);
//        //public ToDoUser? GetById(Guid id) => _users.FirstOrDefault(u => u.Id == id);
//        //public IReadOnlyList<ToDoUser> GetAll() => _users.AsReadOnly();

//        //public void Add(ToDoUser entity)
//        //{
//        //    _users.Add(entity);
//        //    SaveData();
//        //}

//        //public void Update(ToDoUser entity)
//        //{
//        //    var existing = _users.FirstOrDefault(u => u.Id == entity.Id);
//        //    if (existing != null)
//        //    {
//        //        // Обновляем свойства existing на основе entity
//        //        existing.TelegramUserName = entity.TelegramUserName;
//        //        // ... и т.д.
//        //    }
//        //    SaveData();
//        //}

//        //public void Delete(Guid id)
//        //{
//        //    _users.RemoveAll(u => u.Id == id);
//        //    SaveData();
//        //}
//    }
//}
