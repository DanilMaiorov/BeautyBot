using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Mappers
{
    public static class ProcedureModelMapper
    {
        public static IProcedure MapFromModel(ProcedureModel model)
        {
            var procedureEntity = ProcedureFactory.CreateProcedure(model.Subtype, model.Subtype);
            procedureEntity.Id = model.Id;
            return procedureEntity;
        }
        public static ProcedureModel MapToModel(IProcedure entity)
        {
            return new ProcedureModel
            {
                Id = entity.Id,
                Type = Helper.GetBaseProcedureName(entity),
                Subtype = entity.Name,
                Price = entity.Price,
                Duration = entity.Duration,
            };
        }
    }
}
