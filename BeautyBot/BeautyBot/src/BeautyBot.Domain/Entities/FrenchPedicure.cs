using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class FrenchPedicure : Pedicure
    {
        public PedicureType Type { get; } = PedicureType.French;
        public FrenchPedicure() : base("Маникюр френч", 1800, Procedure.Manicure, 120) { }
    }
}