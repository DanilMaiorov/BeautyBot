namespace BeautyBot.src.BeautyBot.Core.Interfaces
{
    public interface IProcedure
    {
        Guid Id { get; }
        string Name { get; }
        decimal Price { get; }
        int Duration { get; }
    }
}