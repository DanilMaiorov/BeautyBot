using BeautyBot.src.BeautyBot.Core.BaseClasses;
using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities.to_the_future
{
    public class Lashes : ProcedureBase
    {
        public Lashes(string name, decimal price, Procedure procedure, int duration) : base(name, price, duration) { }
    }
}
