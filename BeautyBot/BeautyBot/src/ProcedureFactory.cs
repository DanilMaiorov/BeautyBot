using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src
{
    internal class ProcedureFactory
    {
        public static IProcedure CreateProcedure(string procedureName, out IProcedure procedure)
        {
            switch (procedureName)
            {
                case "френч":
                    procedure = new FrenchManicure();
                    break;
                case "гель":
                    procedure = new GelPolishManicure();
                    break;
                default:
                    procedure = new FrenchManicure();
                    break;
            }

            return procedure;
        }
    }
}
