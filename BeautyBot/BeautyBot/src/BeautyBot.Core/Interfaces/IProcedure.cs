using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Core.Interfaces
{
    public interface IProcedure
    {
        Guid Id { get; set; }
        ProcedureBaseType BaseType { get; set; }
        decimal Price { get; set; }
        int Duration { get; set; }
    }
}