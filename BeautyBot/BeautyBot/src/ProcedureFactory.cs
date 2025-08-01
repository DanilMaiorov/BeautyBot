using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src
{
    public static class ProcedureFactory
    {
        private static readonly Dictionary<string, Func<IProcedure, IProcedure>> _procedureCreators =
            new Dictionary<string, Func<IProcedure, IProcedure>>(StringComparer.OrdinalIgnoreCase)
            {
                ["френч"] = current => current is Manicure ? new FrenchManicure() : new FrenchPedicure(),

                ["гель-лак"] = current => current is Manicure ? new GelPolishManicure() : new GelPolishPedicure(),

                ["классический"] = current => current is Manicure ? new ClassicManicure() : new ClassicPedicure(),

                //["newProc"] = _ => new newProc(),
            };

        public static IProcedure CreateProcedure(string procedureName, IProcedure currentStepProcedure)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
                throw new ArgumentException("Procedure name cannot be empty", nameof(procedureName));

            if (_procedureCreators.TryGetValue(procedureName, out var creator))
                return creator(currentStepProcedure);

            throw new ArgumentException($"Unknown procedure: {procedureName}", nameof(procedureName));
        }
    }
}
