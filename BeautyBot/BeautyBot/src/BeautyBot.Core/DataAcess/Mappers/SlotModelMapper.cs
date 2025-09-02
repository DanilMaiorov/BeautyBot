using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Mappers
{
    public static class SlotModelMapper
    {
        public static Slot MapFromModel(SlotModel model)
        {
            return new Slot
            {
                Date = model.Date,
                StartTime = model.StartTime,
                Duration = model.Duration,
                AppointmentId = model.AppointmentId,
            };
        }
        public static SlotModel MapToModel(Slot entity)
        {
            return new SlotModel
            {
                Date = entity.Date,
                StartTime = entity.StartTime,
                Duration = entity.Duration,
                AppointmentId = entity.AppointmentId,
            };
        }
    }
}
