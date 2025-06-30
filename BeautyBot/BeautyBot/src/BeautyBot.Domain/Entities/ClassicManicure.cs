using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    public class ClassicManicure : Manicure
    {
        public ManicureType Type { get; } = ManicureType.French;
        public ClassicManicure() : base("Класссический маникюр", 1000, Procedure.Manicure) { }
    }
}