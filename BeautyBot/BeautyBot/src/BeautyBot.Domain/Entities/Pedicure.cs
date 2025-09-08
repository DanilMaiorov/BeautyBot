using BeautyBot.src.BeautyBot.Core.BaseClasses;
using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Domain.Entities;
using System.Security.AccessControl;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    /// <summary>
    /// Класс маникюра
    /// </summary>
    public class Pedicure : ProcedureBase
    {
        /// <summary>
        /// Флаг необходимости снятия старого педикюра
        /// </summary>
        private bool _withRemove;
        public PedicureType Type { get; }
        public bool WithRemove
        {
            get { return _withRemove; }
            set { _withRemove = value; }
        }
        public Pedicure(PedicureType type, decimal price, int duration, bool withRemove = true)
        {
            BaseType = ProcedureBaseType.Pedicure;
            Price = price;
            Duration = duration;
            Type = type;
            _withRemove = withRemove;
        }
    }
}
