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
        private readonly List<CreateAppointmentTemplate> _createSteps = new List<CreateAppointmentTemplate>();

        public async Task AddStep(IProcedure procedure)
        {
            _createSteps.Add(new CreateAppointmentTemplate(procedure));

            await Task.Delay(1);
        }

        public async Task AddStep(IProcedure procedure, DateTime date)
        {
            _createSteps.Add(new CreateAppointmentTemplate(procedure, date));

            await Task.Delay(1);
        }

        public async Task AddStep(IProcedure procedure, DateTime date, DateTime time)
        {
            _createSteps.Add(new CreateAppointmentTemplate(procedure, date, time));

            await Task.Delay(1);
        }

        public async Task<CreateAppointmentTemplate> GetStep()
        {
            var lastStep = _createSteps.Last();

            await Task.Delay(1);

            return lastStep;
        }

        public async Task<IReadOnlyList<CreateAppointmentTemplate>> GetSteps()
        {
            await Task.Delay(1);
            return _createSteps;
        }
    }
}
