using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class ClassicPedicure : Pedicure
    {
        public ClassicPedicure(string name, decimal price, PedicureType type, int duration) : base(name, price, type, duration) { }
    }
}