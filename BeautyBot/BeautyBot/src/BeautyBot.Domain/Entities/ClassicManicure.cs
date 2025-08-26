using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class ClassicManicure : Manicure
    {
        public ClassicManicure(string name, decimal price, ManicureType type, int duration) : base(name, price, type, duration) { }
    }
}