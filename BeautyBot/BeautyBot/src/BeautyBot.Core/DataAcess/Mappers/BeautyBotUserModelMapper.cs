using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Mappers
{
    public static class BeautyBotUserModelMapper
    {
        public static BeautyBotUser? MapFromModel(BeautyBotUserModel model)
        {
            return model != null ? new BeautyBotUser
            {
                UserId = model.UserId,
                TelegramUserId = model.TelegramUserId,
                TelegramUserName = model.TelegramUserName,
                TelegramUserFirstName = model.TelegramUserFirstName,
                TelegramUserLastName = model.TelegramUserLastName,
                RegisteredAt = model.RegisteredAt,
                Role = model.Role,
            } : null;
        }
        public static BeautyBotUserModel MapToModel(BeautyBotUser entity)
        {
            return new BeautyBotUserModel
            {
                UserId = entity.UserId,
                TelegramUserId = entity.TelegramUserId,
                TelegramUserName = entity.TelegramUserName,
                TelegramUserFirstName = entity.TelegramUserFirstName,
                TelegramUserLastName = entity.TelegramUserLastName,
                RegisteredAt = entity.RegisteredAt,
                Role = entity.Role,
            };
        }
    }
}