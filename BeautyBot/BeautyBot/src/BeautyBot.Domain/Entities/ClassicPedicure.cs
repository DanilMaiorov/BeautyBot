using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class ClassicPedicure : Pedicure
    {
        public ClassicPedicure(decimal price, int duration) : base(PedicureType.Classic, price, duration) { }
    }
}