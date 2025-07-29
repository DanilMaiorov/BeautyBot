using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;

namespace BeautyBot.src.BeautyBot.Domain.Services
{
    public interface ICreateAppointmentTemplate
    {
        Task AddStep(IProcedure procedure);
        Task AddStep(IProcedure procedure, DateOnly date);
        Task AddStep(IProcedure procedure, DateOnly date, TimeOnly time);
        Task<CreateAppointmentTemplate?> GetStep();
        Task<List<CreateAppointmentTemplate>> GetSteps();
        Task RemoveStep();
        Task RefreshSteps();
    }
}
