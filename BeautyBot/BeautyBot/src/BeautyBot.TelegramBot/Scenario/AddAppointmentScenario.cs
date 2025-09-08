using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public class AddAppointmentScenario : IScenario
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ISlotService _slotService;

        private readonly PostgreSqlProcedureRepository _procedureRepository;

        public AddAppointmentScenario(IAppointmentService appointmentService, ISlotService slotService, PostgreSqlProcedureRepository procedureRepository)
        {
            _appointmentService = appointmentService;
            _slotService = slotService;
            _procedureRepository = procedureRepository;
        }

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddAppointment;
        }

        public async Task<ScenarioResponse> HandleMessageAsync(ScenarioContext context, BeautyBotUser user, Chat chat, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    return await HandleInitialStep(context, user, chat, ct);

                case "BaseProcedure":
                    return await HandleBaseProcedureStep(context, chat, ct);

                case "TypeProcedure":
                    return await HandleTypeProcedureStep(context, chat, ct);

                case "DateProcedure":
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
        private async Task<ScenarioResponse> HandleInitialStep(ScenarioContext context, BeautyBotUser user, Chat chat, CancellationToken ct)
        {
            await Task.Delay(1, ct);

            context.Data["User"] = user;

            context.CurrentStep = "BaseProcedure";

            return new ScenarioResponse(ScenarioResult.Transition, chat)
            {
                Message = "Куда записываемся?",
                Keyboard = Keyboards.secondStep
            };
        }
        private async Task<ScenarioResponse> HandleBaseProcedureStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            await Task.Delay(1, ct);

            var procedureData = new Dictionary<string, (string Message, ReplyMarkup Keyboard)>
            {
                [Constants.Manicure] = (Message: Constants.ChooseManicure, Keyboard: Keyboards.thirdManicureStep),
                [Constants.Pedicure] = (Message: Constants.ChoosePedicure, Keyboard: Keyboards.thirdPedicureStep),
                //[Constants.Eyebrows] = (Message: "Выберите форму бровей", Keyboard: Keyboards.eyebrowsStep),
                //[Constants.Lashes] = (Message: "Выберите вид ресничек", Keyboard: Keyboards.lashesStep)
            };

            if (!procedureData.TryGetValue((string)context.Data["BaseProcedure"], out var data))
                throw new Exception("Неизвестная процедура");

            var (message, keyboard) = data;

            context.CurrentStep = "TypeProcedure";

            return new ScenarioResponse(ScenarioResult.Transition, chat)
            {
                Message = message,
                Keyboard = keyboard,
            };
        }
        private async Task<ScenarioResponse> HandleTypeProcedureStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

            context.CurrentStep = "DateProcedure";

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
        private async Task<ScenarioResponse> HandleChooseDateStep(ScenarioContext context, Chat chat,CancellationToken ct)
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

            return new ScenarioResponse(ScenarioResult.Transition, chat)
            {
                Message = $"Выбранное время - {(string)context.Data["TimeProcedure"]}\n\nВерно?",
                Keyboard = Keyboards.approveTime
            };
        }
        private async Task<ScenarioResponse> HandleApproveTimeStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            await _procedureRepository.Add((IProcedure)context.Data["TypeProcedure"], ct);

            var newAppointment = await _appointmentService.AddAppointment(
                (BeautyBotUser)context.Data["User"],
                (IProcedure)context.Data["TypeProcedure"],
                (DateOnly)context.Data["DateProcedure"],
                (TimeOnly)context.Data["TimeProcedure"],
                ct);

            await _slotService.UpdateSlotFromAppointment(newAppointment, ct);

            return new ScenarioResponse(ScenarioResult.Completed, chat)
            {
                Message = $"Вы успешно записаны🤗\n\nЖдём Вас {context.Data["DateProcedure"]} в {context.Data["TimeProcedure"]}\n\nПо адресу г. Екатеринбург ул. Ленина 1, офис 101\n\nПрекрасного дня ☀️",
                Keyboard = Keyboards.firstStep
            };
        }
    }
}

