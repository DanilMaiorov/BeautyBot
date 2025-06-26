using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class GelPolishManicure : Manicure
    {
        public ManicureType Type { get; } = ManicureType.GelPolish;
        public GelPolishManicure() : base("Маникюр гель-лак", 1500, Procedure.Manicure) { }
    }
}
