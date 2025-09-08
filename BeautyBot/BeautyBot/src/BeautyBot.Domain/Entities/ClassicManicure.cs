using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class ClassicManicure : Manicure
    {
        public ClassicManicure(decimal price, int duration) : base(ManicureType.Classic, price, duration) { }
    }
}