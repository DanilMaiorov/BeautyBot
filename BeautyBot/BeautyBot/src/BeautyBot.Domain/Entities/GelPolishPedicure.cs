using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class GelPolishPedicure : Pedicure
    {
        public GelPolishPedicure(decimal price, int duration) : base(PedicureType.GelPolish, price, duration) { }
    }
}
