using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class FrenchManicure : Manicure
    {
        public int TypeKey { get; } = 1;
        public FrenchManicure() : base("Маникюр френч", 1800, Procedure.Manicure) { }
    }
}
