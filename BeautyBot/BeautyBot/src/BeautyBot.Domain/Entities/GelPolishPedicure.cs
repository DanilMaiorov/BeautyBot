using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class GelPolishPedicure : Pedicure
    {
        public PedicureType Type { get; } = PedicureType.GelPolish;
        public GelPolishPedicure() : base("Педикюр гель-лак", 1400, Procedure.Manicure, 120) { }
    }
}
