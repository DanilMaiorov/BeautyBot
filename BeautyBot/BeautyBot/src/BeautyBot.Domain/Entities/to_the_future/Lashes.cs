using BeautyBot.src.BeautyBot.Core.BaseClasses;
using BeautyBot.src.BeautyBot.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Domain.Entities.to_the_future
{
    public class Lashes : ProcedureBase
    {
        public Lashes(string name, decimal price, Procedure procedure) : base(name, price, procedure) { }
    }
}
