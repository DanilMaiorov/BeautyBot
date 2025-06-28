using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory
{
    public class InMemoryCreateAppointmentTemplate : ICreateAppointmentTemplate
    {
        private readonly List<CreateAppointmentTemplate> _steps = new List<CreateAppointmentTemplate>();

        public async Task AddStep(IProcedure procedure)
        {
            _steps.Add(new CreateAppointmentTemplate(procedure));

            await Task.Delay(1);
        }

        public async Task AddStep(IProcedure procedure, string date)
        {
            _steps.Add(new CreateAppointmentTemplate(procedure, date));

            await Task.Delay(1);
        }

        public async Task AddStep(IProcedure procedure, string date, string time)
        {
            _steps.Add(new CreateAppointmentTemplate(procedure, date, time));

            await Task.Delay(1);
        }

        public async Task<CreateAppointmentTemplate?> GetStep()
        {
            if(_steps.Count > 0 )
                return _steps.Last();

            await Task.Delay(1);

            return null;
        }

        public async Task<IReadOnlyList<CreateAppointmentTemplate>> GetSteps()
        {
            await Task.Delay(1);
            return _steps;
        }

        public async Task RemoveStep()
        {
            await Task.Delay(1);
            _steps.RemoveAt(_steps.Count - 1);
        }

        public async Task RefreshSteps()
        {
            await Task.Delay(1);
            _steps.Clear();
        }
    }
}
