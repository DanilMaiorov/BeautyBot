using BeautyBot.src.BeautyBot.Core.BaseClasses;
using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities.to_the_future
{
    public class Brows : ProcedureBase
    {
        public bool WithTattoo;
        public Brows(string name, decimal price, ProcedureBaseType procedure, int duration) : base(name, price, duration)
        {
            WithTattoo = false;
        }
    }
}
