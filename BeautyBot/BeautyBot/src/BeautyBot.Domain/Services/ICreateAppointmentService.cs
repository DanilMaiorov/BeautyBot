using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface ICreateAppointmentService
    {
        Task AddStep(IProcedure procedure);
        Task AddStep(IProcedure procedure, DateTime date);
        Task AddStep(IProcedure procedure, DateTime date, DateTime time);

        Task<CreateAppointmentTemplate> GetStep();

        Task<IReadOnlyList<CreateAppointmentTemplate>> GetSteps();
    }
}
