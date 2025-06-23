using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class GelPolishManicure : Manicure
    {
        public int TypeKey { get; } = 2;
        public GelPolishManicure() : base("Маникюр гель-лак", 1500, Procedure.Manicure) { }
    }
}
