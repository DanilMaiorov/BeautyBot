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
                case "french":
                    procedure = new FrenchManicure();
                    break;
                case "gelpolish":
                    procedure = new GelPolishManicure();
                    break;
                case "classic":
                    procedure = new ClassicManicure();
                    break;
                case "french1":
                    procedure = new GelPolishPedicure();
                    break;
                case "classic1":
                    procedure = new ClassicPedicure();
                    break;
                default:
                    throw new ArgumentException("Что-то пошло не так");
            }

            return procedure;
        }
    }
}
