using BeautyBot.src.BeautyBot.Core.Enums;
using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class CreateAppointmentService : ICreateAppointmentService
    {

        private readonly ICreateAppointmentTemplate _createAppointmentTemplate;

        public async Task AddStep(IProcedure procedure)
        {
            await _createAppointmentTemplate.AddStep(procedure);
        }

        public async Task AddStep(IProcedure procedure, string date)
        {
            await _createAppointmentTemplate.AddStep(procedure, date);
        }

        public async Task AddStep(IProcedure procedure, string date, string time)
        {
            await _createAppointmentTemplate.AddStep(procedure, date, time);
        }

        public async Task<CreateAppointmentTemplate> GetStep()
        {
            return await _createAppointmentTemplate.GetStep();
        }

        public async Task<IReadOnlyList<CreateAppointmentTemplate>> GetSteps()
        {
            return await _createAppointmentTemplate.GetSteps();
        }


        public async Task RemoveStep()
        {
            await _createAppointmentTemplate.RemoveStep();
        }


        public CreateAppointmentService(ICreateAppointmentTemplate createAppointmentTemplate)
        {
            _createAppointmentTemplate = createAppointmentTemplate;
        }

    }
}
