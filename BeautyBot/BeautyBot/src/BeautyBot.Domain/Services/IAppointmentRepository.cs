using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    // Репозиторий для записей (назначений)
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        //IReadOnlyList<Appointment> GetAllByUserId(Guid userId);
        //IReadOnlyList<Appointment> GetActiveByUserId(Guid userId);
    }
}
