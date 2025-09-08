using BeautyBot.src.BeautyBot.Core.BaseClasses;
using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Domain.Entities;
using System.Security.AccessControl;

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
        public Manicure(ManicureType type, decimal price, int duration, bool withRemove = true)
        {
            BaseType = ProcedureBaseType.Manicure;
            Price = price;
            Duration = duration;
            Type = type;
            _withRemove = withRemove;
        }
    }    
}


