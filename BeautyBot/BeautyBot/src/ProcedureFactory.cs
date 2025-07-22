using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src
{
    //internal class ProcedureFactory
    //{
    //    public static IProcedure CreateProcedure(string procedureName, IProcedure currentStepProcedure, out IProcedure procedure)
    //    {
    //        switch (procedureName)
    //        {
    //            case "french":
    //                if(currentStepProcedure is Manicure)
    //                {
    //                    procedure = new FrenchManicure();
    //                }
    //                //if (currentStepProcedure is Pedicure)
    //                else
    //                {
    //                    procedure = new FrenchPedicure();
    //                }
    //                break;
    //            case "gelpolish":
    //                procedure = new GelPolishManicure();
    //                break;
    //            case "classic":
    //                procedure = new ClassicManicure();
    //                break;
    //            case "french1":
    //                procedure = new GelPolishPedicure();
    //                break;
    //            case "classic1":
    //                procedure = new ClassicPedicure();
    //                break;
    //            default:
    //                throw new ArgumentException("Что-то пошло не так");
    //        }

    //        return procedure;
    //    }
    //}


    public static class ProcedureFactory
    {
        private static readonly Dictionary<string, Func<IProcedure, IProcedure>> _procedureCreators =
            new Dictionary<string, Func<IProcedure, IProcedure>>(StringComparer.OrdinalIgnoreCase)
            {
                ["french"] = current => current is Manicure ? new FrenchManicure() : new FrenchPedicure(),

                ["gelpolish"] = current => current is Manicure ? new GelPolishManicure() : new GelPolishPedicure(),

                ["classic"] = current => current is Manicure ? new ClassicManicure() : new ClassicPedicure(),

                //["newProc"] = _ => new GelPolishPedicure(),

                //["classic1"] = _ => new ClassicPedicure()
            };

        public static IProcedure CreateProcedure(string procedureName, IProcedure currentStepProcedure)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
                throw new ArgumentException("Procedure name cannot be empty", nameof(procedureName));

            if (_procedureCreators.TryGetValue(procedureName, out var creator))
            {
                return creator(currentStepProcedure);
            }

            throw new ArgumentException($"Unknown procedure: {procedureName}", nameof(procedureName));
        }
    }
}
