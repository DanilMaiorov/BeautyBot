using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src
{
    public static class ProcedureFactory
    {
        private static readonly Dictionary<string, Func<string, IProcedure>> _procedureCreators =
            new Dictionary<string, Func<string, IProcedure>>(StringComparer.OrdinalIgnoreCase)
            {
                ["френч"] = baseProcedureName => baseProcedureName == Constants.Manicure
                    ? new FrenchManicure(Constants.FrenchManicure, Prices.ClassicManicure, ManicureType.French, 120) 
                    : new FrenchPedicure(Constants.FrenchPedicure, Prices.ClassicManicure, PedicureType.French, 120),

                ["гель-лак"] = baseProcedureName => baseProcedureName == Constants.Manicure
                    ? new GelPolishManicure(Constants.GelPolishManicure, Prices.ClassicManicure, ManicureType.GelPolish, 120) 
                    : new GelPolishPedicure(Constants.GelPolishPedicure, Prices.ClassicManicure, PedicureType.GelPolish, 120),

                ["классический"] = baseProcedureName => baseProcedureName == Constants.Manicure
                    ? new ClassicManicure(Constants.ClassicManicure, Prices.ClassicManicure, ManicureType.Classic, 60) 
                    : new ClassicPedicure(Constants.ClassicPedicure, Prices.ClassicManicure, PedicureType.Classic, 60),
            };

        public static IProcedure CreateProcedure(string procedureName, string currentStepProcedure)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
                throw new ArgumentException("Procedure name cannot be empty", nameof(procedureName));

            if (_procedureCreators.TryGetValue(procedureName, out var creator))
                return creator(currentStepProcedure);

            throw new ArgumentException($"Unknown procedure: {procedureName}", nameof(procedureName));
        }
    }
}
