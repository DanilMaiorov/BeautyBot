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
        Task AddStep(IProcedure procedure, string date);
        Task AddStep(IProcedure procedure, string date, string time);

        Task RemoveStep();

        Task<CreateAppointmentTemplate> GetStep();

        Task<IReadOnlyList<CreateAppointmentTemplate>> GetSteps();

        Task RefreshSteps();
    }
}
