using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public class CancelAppointmentScenario : IScenario
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ISlotService _slotService;

        private readonly PostgreSqlProcedureRepository _procedureRepository;

        public CancelAppointmentScenario(IAppointmentService appointmentService, ISlotService slotService, PostgreSqlProcedureRepository procedureRepository)
        {
            _appointmentService = appointmentService;
            _slotService = slotService;
            _procedureRepository = procedureRepository;
        }

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.CancelAppointment;
        }

        public async Task<ScenarioResponse> HandleMessageAsync(ScenarioContext context, BeautyBotUser user, Chat chat, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    return await HandleApproveStep(context, chat, ct);

                case "CancelAppointment":
                    return await HandleCancelStep(context, chat, ct);

                default:
                    return new ScenarioResponse(ScenarioResult.Transition, chat)
                    {
                        Message = "Неизвестный шаг сценария",
                        Keyboard = Keyboards.firstStep
                    };
            }
        }
        private async Task<ScenarioResponse> HandleApproveStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            await Task.Delay(1, ct);

            context.CurrentStep = "CancelAppointment";

            return new ScenarioResponse(ScenarioResult.Transition, chat)
            {
                Message = "Подтверждаете отмену записи?",
                Keyboard = Keyboards.GetApproveCancelAppointmentKeyboard()
            };
        }
        private async Task<ScenarioResponse> HandleCancelStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            return new ScenarioResponse(ScenarioResult.Completed, chat)
            {
                Message = "Ваша запись отменена. Вы в в главном меню. Что хотите сделать?",
                Keyboard = Keyboards.firstStep,
            };
        }
    }
}

