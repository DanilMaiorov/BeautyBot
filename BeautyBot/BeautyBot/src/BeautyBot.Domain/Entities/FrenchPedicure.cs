using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class FrenchPedicure : Pedicure
    {
        public FrenchPedicure(decimal price, int duration) : base(PedicureType.French, price, duration) { }
    }
}