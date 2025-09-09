using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public class EditAppointmentScenario : IScenario
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ISlotService _slotService;

        private readonly PostgreSqlProcedureRepository _procedureRepository;

        public EditAppointmentScenario(IAppointmentService appointmentService, ISlotService slotService, PostgreSqlProcedureRepository procedureRepository)
        {
            _appointmentService = appointmentService;
            _slotService = slotService;
            _procedureRepository = procedureRepository;
        }

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.EditAppointment;
        }

        public async Task<ScenarioResponse> HandleMessageAsync(ScenarioContext context, BeautyBotUser user, Chat chat, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    return await HandleEditTypeStep(context, chat, ct);

                case "DateProcedure":
                    return await HandleDateProcedureStep(context, chat, ct);

                case "ChooseDateProcedure":
                    return await HandleChooseDateStep(context, chat, ct);

                case "ApproveDateProcedure":
                    return await HandleApproveDateStep(context, chat, ct);

                case "TimeProcedure":
                    return await HandleChooseTimeStep(context, chat, ct);

                case "ApproveTimeProcedure":
                    return await HandleApproveTimeStep(context, chat, ct);

                default:
                    return new ScenarioResponse(ScenarioResult.Transition, chat)
                    {
                        Message = "Неизвестный шаг сценария",
                        Keyboard = Keyboards.firstStep
                    };
            }
        }

        private async Task<ScenarioResponse> HandleEditTypeStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            if (!context.Data.TryGetValue("Appointment", out var appointmentObj))
                throw new KeyNotFoundException("Не найден контекст даты");

            if (appointmentObj is not Appointment appointment)
                throw new InvalidCastException($"Ожидался Appointment, получен {appointmentObj?.GetType().Name ?? "null"}");
           
            return new ScenarioResponse(ScenarioResult.Transition, chat)
            {
                Keyboard = Keyboards.AppointmentEditItemKeyboard(appointment),
                IsEdit = true
            };
        }

        private async Task<ScenarioResponse> HandleDateProcedureStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

            context.CurrentStep = "ChooseDateProcedure";

            var messagesToSend = new List<(string messages, ReplyMarkup keyboards)>
            {
                ("Выберите дату", Keyboards.cancelOrBack),
                ("✖ - означает, что на выбранную дату нет свободных слотов", Keyboards.DaySlotsKeyboard(DateTime.Today, unavailableSlots))
            };

            return new ScenarioResponse(ScenarioResult.Transition, chat)
            {
                Messages = messagesToSend
            };
        }
        private async Task<ScenarioResponse> HandleChooseDateStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            context.DataHistory.Push(context.Data["DateProcedure"].ToString());

            await Task.Delay(1, ct);

            context.CurrentStep = "ApproveDateProcedure";

            return new ScenarioResponse(ScenarioResult.Transition, chat)
            {
                Message = $"Выбранная дата - {context.Data["DateProcedure"]}\n\nВерно?",
                Keyboard = Keyboards.approveDate
            };
        }
        private async Task<ScenarioResponse> HandleApproveDateStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            if (!context.Data.TryGetValue("DateProcedure", out var dateObj))
                throw new KeyNotFoundException("Не найден контекст даты");

            if (dateObj is not DateOnly date)
                throw new InvalidCastException($"Ожидался DateOnly, получен {dateObj?.GetType().Name ?? "null"}");

            var slots = await _slotService.GetSlotsByDate(date, ct);

            context.CurrentStep = "TimeProcedure";

            return new ScenarioResponse(ScenarioResult.Transition, chat)
            {
                Message = "Выберите время",
                Keyboard = Keyboards.TimeSlotsKeyboard(slots)
            };
        }
        private async Task<ScenarioResponse> HandleChooseTimeStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            context.CurrentStep = "ApproveTimeProcedure";

            var time = (TimeOnly)context.Data["TimeProcedure"];

            return new ScenarioResponse(ScenarioResult.Transition, chat)
            {
                Message = $"Выбранное время - {time.ToString("HH:mm")}\n\nВерно?",
                Keyboard = Keyboards.approveTime
            };
        }
        private async Task<ScenarioResponse> HandleApproveTimeStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            if (!context.Data.TryGetValue("Appointment", out var appointmentObj))
                throw new KeyNotFoundException("Не найден контекст даты");

            if (appointmentObj is not Appointment appointment)
                throw new InvalidCastException($"Ожидался Appointment, получен {appointmentObj?.GetType().Name ?? "null"}");

            await _slotService.ResetSlotFromAppointment(appointment, ct);

            var dateProcedure = (DateOnly)context.Data["DateProcedure"];
            var timeProcedure = (TimeOnly)context.Data["TimeProcedure"];

            appointment.AppointmentDate = dateProcedure.ToDateTime(timeProcedure);

            await _appointmentService.EditAppointment(appointment.Id, appointment.AppointmentDate, ct);

            await _slotService.UpdateSlotFromAppointment(appointment, ct);

            return new ScenarioResponse(ScenarioResult.Completed, chat)
            {
                Message = $"Запись успешно изменена💜\n\nЖдём Вас {dateProcedure} в {timeProcedure}\n\nПо адресу г. Екатеринбург ул. Ленина 1, офис 101\n\nПрекрасного дня ☀️",
                Keyboard = Keyboards.firstStep
            };
        }
    }
}
