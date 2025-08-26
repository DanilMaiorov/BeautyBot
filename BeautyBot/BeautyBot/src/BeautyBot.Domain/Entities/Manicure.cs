using BeautyBot.src.BeautyBot.Core.BaseClasses;
using BeautyBot.src.BeautyBot.Core.Enums;

namespace BeautyBot.src.BeautyBot.Domain.Entities
{
    /// <summary>
    /// Класс маникюра
    /// </summary>
    public class Manicure : ProcedureBase
    {
        /// <summary>
        /// Флаг необходимости снятия старого маникюра
        /// </summary>
        private bool _withRemove;
        public ManicureType Type { get; }
        public bool WithRemove
        {
            get { return _withRemove; }
            set { _withRemove = value; }
        }
        public Manicure(bool withRemove = true)
        {
            _withRemove = withRemove;
        }
        public Manicure(string name, decimal price, ManicureType type, int duration, bool withRemove = true) : base(name, price, duration) 
        {
            Type = type;
            _withRemove = withRemove;
        }
    }    
}
