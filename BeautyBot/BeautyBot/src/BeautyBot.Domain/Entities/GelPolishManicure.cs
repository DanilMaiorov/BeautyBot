using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class GelPolishManicure : Manicure
    {
        public GelPolishManicure(decimal price, int duration) : base(ManicureType.GelPolish, price, duration) { }
    }
}
