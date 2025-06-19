using BeautyBot.src.BeautyBot.Core.BaseClasses;
using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool WithRemove
        {
            get { return _withRemove; }
            set { _withRemove = value; }
        }

        public Manicure(string name, decimal price, Procedure procedure, bool withRemove = true) : base(name, price, procedure) {

            _withRemove = withRemove;
        }
    }    
}
