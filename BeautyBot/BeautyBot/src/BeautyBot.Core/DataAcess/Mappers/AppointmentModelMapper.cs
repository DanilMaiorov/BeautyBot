using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Mappers
{
    public static class AppointmentModelMapper
    {
        public static Appointment MapFromModel(AppointmentModel model)
        {
            return new Appointment (
                BeautyBotUserModelMapper.MapFromModel(model.User),
                ProcedureModelMapper.MapFromModel(model.Procedure),
                DateOnly.FromDateTime(model.AppointmentDate),
                TimeOnly.FromDateTime(model.AppointmentDate)
                )
            {
                Id = model.Id,
                CreatedAt = model.CreatedAt,
                StateChangedAt = model.StateChangedAt,
                AppointmentDuration = model.AppointmentDuration,
                State = model.State,
            };
        }
        public static AppointmentModel MapToModel(Appointment entity)
        {
            var appointmentModel = ProcedureModelMapper.MapToModel(entity.Procedure);

            return new AppointmentModel
            {
                Id = entity.Id,
                UserId = entity.User.UserId,
                User = BeautyBotUserModelMapper.MapToModel(entity.User),
                ProcedureId = appointmentModel.Id,
                Procedure = appointmentModel,
                CreatedAt = entity.CreatedAt,
                StateChangedAt = entity.StateChangedAt,
                AppointmentDate = entity.AppointmentDate,
                AppointmentDuration = entity.AppointmentDuration,
                State = entity.State,
            };
        }
    }
}