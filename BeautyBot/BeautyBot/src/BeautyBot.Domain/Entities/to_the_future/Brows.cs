using BeautyBot.src.BeautyBot.Core.BaseClasses;
using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities.to_the_future
{
    public class Brows : ProcedureBase
    {
        public bool WithTattoo;
        public Brows(string name, decimal price, Procedure procedure) : base(name, price, procedure)
        {

            WithTattoo = false;

        }
    }
}
