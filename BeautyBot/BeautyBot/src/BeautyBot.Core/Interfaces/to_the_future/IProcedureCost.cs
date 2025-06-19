using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Core.Interfaces.to_the_future
{
    // Интерфейс для передачи только нужных данных для расчета
    public interface IProcedureCost
    {
        Guid Id { get; }
        decimal Price { get; }

        // докинуть поля по необходимости
    }
}
