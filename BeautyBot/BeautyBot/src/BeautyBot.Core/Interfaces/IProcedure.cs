namespace BeautyBot.src.BeautyBot.Core.Interfaces
{
    public interface IProcedure
    {
        Guid Id { get; set; }
        string Name { get; set; }
        decimal Price { get; set; }
        int Duration { get; set; }
    }
}