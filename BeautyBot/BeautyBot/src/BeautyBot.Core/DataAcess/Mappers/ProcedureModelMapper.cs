using BeautyBot.src.BeautyBot.Core.DataAcess.Models;
using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Mappers
{
    public static class ProcedureModelMapper
    {
        public static IProcedure MapFromModel(ProcedureModel model)
        {
            IProcedure procedureEntity;

            var baseType = Helper.GetEnumValueOrDefault<ProcedureBaseType>(model.Type);

            switch (baseType)
            {
                case ProcedureBaseType.Manicure:
                    var manicureType = Helper.GetEnumValueOrDefault<ManicureType>(model.Subtype);
                    procedureEntity = ProcedureFactory.CreateProcedure(baseType, manicureType);
                    break;

                case ProcedureBaseType.Pedicure:
                    var pedicureType = Helper.GetEnumValueOrDefault<PedicureType>(model.Subtype);
                    procedureEntity = ProcedureFactory.CreateProcedure(baseType, pedicureType);
                    break;

                default:
                    throw new NotSupportedException($"Unsupported procedure type: {model.Type}");
            }

            procedureEntity.Id = model.Id;

            return procedureEntity;
        }
        public static ProcedureModel MapToModel(IProcedure entity)
        {
            return new ProcedureModel
            {
                Id = entity.Id,
                Type = entity.BaseType.ToString(),
                Subtype = Helper.GetSubtypeName(entity),
                Price = entity.Price,
                Duration = entity.Duration,
            };
        }
    }
}
