using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;

namespace BeautyBot.src.BeautyBot.Application.Services
{
    public class CreateAppointmentService : ICreateAppointmentService
    {

        private readonly ICreateAppointmentTemplate _createAppointmentTemplate;

        public async Task AddStep(IProcedure procedure)
        {
            await _createAppointmentTemplate.AddStep(procedure);
        }

        public async Task AddStep(IProcedure procedure, DateOnly date)
        {
            await _createAppointmentTemplate.AddStep(procedure, date);
        }

        public async Task AddStep(IProcedure procedure, DateOnly date, TimeOnly time)
        {
            await _createAppointmentTemplate.AddStep(procedure, date, time);
        }

        public async Task<CreateAppointmentTemplate> GetStep()
        {
            return await _createAppointmentTemplate.GetStep();
        }

        public async Task<List<CreateAppointmentTemplate>> GetSteps()
        {
            return await _createAppointmentTemplate.GetSteps();
        }

        public async Task RemoveStep()
        {
            await _createAppointmentTemplate.RemoveStep();
        }

        public async Task RefreshSteps()
        {
            await _createAppointmentTemplate.RefreshSteps();
        }

        public CreateAppointmentService(ICreateAppointmentTemplate createAppointmentTemplate)
        {
            _createAppointmentTemplate = createAppointmentTemplate;
        }
    }
}
