using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class FrenchManicure : Manicure
    {
        public FrenchManicure(decimal price, int duration) : base(ManicureType.French, price, duration) { }
    }
}