namespace BeautyBot.src.BeautyBot.Core.Interfaces
{
    public interface IProcedure
    {
        public Guid Id { get; }
        public string Name { get; }
        public decimal Price { get; }
        public int Duration { get; }
    }
}