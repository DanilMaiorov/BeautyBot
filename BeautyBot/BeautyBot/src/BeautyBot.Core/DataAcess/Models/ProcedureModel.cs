using LinqToDB.Mapping;

namespace BeautyBot.src.BeautyBot.Core.DataAcess.Models
{
    [Table("Procedures")]
    public class ProcedureModel
    {
        [Column("Id"), PrimaryKey]
        public Guid Id { get; set; }

        [Column("Type"), NotNull]
        public required string Type { get; set; }

        [Column("Subtype"), NotNull]
        public required string Subtype { get; set; }

        [Column("Price"), NotNull]
        public decimal Price { get; set; }

        [Column("Duration"), NotNull]
        public int Duration { get; set; }
    }
}
