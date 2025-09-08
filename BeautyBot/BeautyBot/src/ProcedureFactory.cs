using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src
{
    public static class ProcedureFactory
    {
        // Словарь для маникюра
        private static readonly Dictionary<ManicureType, Func<IProcedure>> _manicureCreators =
            new Dictionary<ManicureType, Func<IProcedure>>
            {
                { ManicureType.French, () => new FrenchManicure(Prices.ClassicManicure, 120) },
                { ManicureType.GelPolish, () => new GelPolishManicure(Prices.ClassicManicure, 120) },
                { ManicureType.Classic, () => new ClassicManicure(Prices.ClassicManicure, 60) },
            };

        // Словарь для педикюра
        private static readonly Dictionary<PedicureType, Func<IProcedure>> _pedicureCreators =
            new Dictionary<PedicureType, Func<IProcedure>>
            {
                { PedicureType.French, () => new FrenchPedicure(Prices.ClassicManicure, 120) },
                { PedicureType.GelPolish, () => new GelPolishPedicure(Prices.ClassicManicure, 120) },
                { PedicureType.Classic, () => new ClassicPedicure(Prices.ClassicManicure, 60) },
            };

        public static IProcedure CreateProcedure(ProcedureBaseType baseType, ManicureType manicureType)
        {
            if (baseType == ProcedureBaseType.Manicure)
                return _manicureCreators[manicureType]();

            throw new ArgumentException($"Неизвестный тип маникюра: {manicureType}", nameof(manicureType));
        }

        public static IProcedure CreateProcedure(ProcedureBaseType baseType, PedicureType pedicureType)
        {
            if (baseType == ProcedureBaseType.Pedicure)
                return _pedicureCreators[pedicureType]();

            throw new ArgumentException($"Неизвестный тип педикюра: {pedicureType}", nameof(pedicureType));
        }

        public static IProcedure CreateProcedure(string typeProcedureInput, string baseProcedureInput)
        {
            var normalizedInput = Helper.NormalizeProcedureTypeName(typeProcedureInput);

            switch (baseProcedureInput)
            {
                case "Маникюр":
                    var manicureType = Helper.GetEnumValueOrDefault<ManicureType>(normalizedInput);
                    if (manicureType == ManicureType.None)
                        throw new ArgumentException("Неверный тип маникюра");
                    return ProcedureFactory.CreateProcedure(ProcedureBaseType.Manicure, manicureType);

                case "Педикюр":
                    var pedicureType = Helper.GetEnumValueOrDefault<PedicureType>(normalizedInput);
                    if (pedicureType == PedicureType.None)
                        throw new ArgumentException("Неверный тип педикюра");
                    return ProcedureFactory.CreateProcedure(ProcedureBaseType.Pedicure, pedicureType);

                default:
                    throw new ArgumentException("Неизвестный тип базовой процедуры");
            }
        }
    }
}
