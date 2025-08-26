using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using LinqToDB.Mapping;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Models
{
    internal class ModelMapper
    {
        public static BeautyBotUser MapFromModel(BeautyBotUserModel model)
        {
            return new BeautyBotUser
            {
                UserId = model.UserId,
                TelegramUserId = model.TelegramUserId,
                TelegramUserName = model.TelegramUserName,
                RegisteredAt = model.RegisteredAt,
            };
        }
        public static BeautyBotUserModel MapToModel(BeautyBotUser entity)
        {
            return new BeautyBotUserModel
            {
                UserId = entity.UserId,
                TelegramUserId = entity.TelegramUserId,
                TelegramUserName = entity.TelegramUserName,
                RegisteredAt = entity.RegisteredAt,
            };
        }
        public class Appointment
        {
            public Guid Id { get; set; }
            public BeautyBotUser User { get; set; }
            public IProcedure Procedure { get; set; } // храню интерфейс или конкретный класс
            public DateTime CreatedAt { get; set; }
            public DateTime StateChangedAt { get; set; }
            public DateTime AppointmentDate { get; set; }
            public int AppointmentDuration { get; set; }
            public AppointmentState State { get; set; }
        }
            public static Appointment MapFromModel(AppointmentModel model)
            {
                return new Appointment
                {
                    Id = model.Id,
                    User = model.User != null ? MapFromModel(model.User) : null,
                    //Procedure = model.Name,
                    CreatedAt = model.CreatedAt,
                    StateChangedAt = model.StateChangedAt,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentDuration = model.AppointmentDuration,
                    State = model.State,
                };
        }
        public static AppointmentModel MapToModel(Appointment entity)
        {
            return new AppointmentModel
            {
                Id = entity.Id,
                UserId = entity.User.UserId,
                User = null!,
                ProcedureId = entity.Procedure.Id,
                CreatedAt = entity.CreatedAt,
                StateChangedAt = entity.StateChangedAt,
                AppointmentDuration = entity.AppointmentDuration,
                State = entity.State
            };
        }
    }
}
