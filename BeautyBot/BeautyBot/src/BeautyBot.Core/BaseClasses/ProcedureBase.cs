using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Core.Interfaces.to_the_future;

namespace BeautyBot.src.BeautyBot.Core.BaseClasses
{
    /// <summary>
    /// Базовый класс процедуры
    /// </summary>
    public abstract class ProcedureBase : IProcedure //,IProcedureCost
    {
        public Guid Id { get; protected set; }
        public Procedure Procedure { get; protected set; }
        public string Name { get; protected set; }
        public decimal Price { get; protected set; }

        protected ProcedureBase(string name, decimal price, Procedure procedure)
        {
            Id = Guid.NewGuid();
            Procedure = procedure;
            Name = name;
            Price = price;
        }
    }
}
