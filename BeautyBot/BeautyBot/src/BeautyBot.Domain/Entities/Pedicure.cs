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
        public bool WithRemove
        {
            get { return _withRemove; }
            set { _withRemove = value; }
        }
        public Pedicure(bool withRemove = true)
        {
            _withRemove = withRemove;
        }
        public Pedicure(string name, decimal price, Procedure procedure, int duration, bool withRemove = true) : base(name, price, procedure, duration)
        {
            _withRemove = withRemove;
        }
    }
}
