using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class ClassicPedicure : Pedicure
    {
        public PedicureType Type { get; } = PedicureType.Classic;
        public ClassicPedicure() : base("Класссический педикюр", 1000, Procedure.Pedicure, 60) { }
    }
}