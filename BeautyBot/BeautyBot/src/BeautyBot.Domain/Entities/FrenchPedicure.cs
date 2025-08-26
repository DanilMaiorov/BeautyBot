using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class FrenchPedicure : Pedicure
    {
        public FrenchPedicure(string name, decimal price, PedicureType type, int duration) : base(name, price, type, duration) { }
    }
}