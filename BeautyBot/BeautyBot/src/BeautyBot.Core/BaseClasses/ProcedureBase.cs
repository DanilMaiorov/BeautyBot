using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;

namespace BeautyBot.src.BeautyBot.Core.BaseClasses
{
    /// <summary>
    /// Базовый класс процедуры
    /// </summary>
    public abstract class ProcedureBase : IProcedure //,IProcedureCost
    {
        public Guid Id { get; set; }
        public ProcedureBaseType BaseType { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        protected ProcedureBase()
        {
            Id = Guid.NewGuid();
        }
        protected ProcedureBase(ProcedureBaseType baseType, decimal price, int duration) : this()
        {
            BaseType = baseType;
            Price = price;
            Duration = duration;
        }
    }
}
