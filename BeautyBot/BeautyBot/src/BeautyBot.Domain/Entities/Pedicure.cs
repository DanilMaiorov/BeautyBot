using BeautyBot.src.BeautyBot.Core.BaseClasses;
using BeautyBot.src.BeautyBot.Core.Enums;

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
        public Pedicure(bool withRemove = true)
        {
            _withRemove = withRemove;
        }
        public Pedicure(string name, decimal price, PedicureType type, int duration, bool withRemove = true) : base(name, price, duration)
        {
            Type = type;
            _withRemove = withRemove;
        }
    }
}
