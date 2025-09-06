using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using static LinqToDB.Reflection.Methods.LinqToDB.Insert;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<ScenarioResponse> HandleMessageAsync(ScenarioContext context, MessageData messageData, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    return await HandleInitialStep(context, messageData.User, messageData.Chat, ct);

                case "BaseProcedure":
                    return await HandleBaseProcedureStep(context, messageData.Chat, messageData.UserInput, ct);

                case "TypeProcedure":
                    return await HandleTypeProcedureStep(context, messageData.Chat, messageData.UserInput, ct);

                case "DateProcedure":
                    return await HandleChooseDateStep(context, messageData.Chat, messageData.UserInput, ct);

                case "ApproveDateProcedure":
                    return await HandleApproveDateStep(context, messageData.Chat, ct);

                case "TimeProcedure":
                    return await HandleChooseTimeStep(context, messageData.Chat, messageData.UserInput, ct);

                case "ApproveTimeProcedure":
                    return await HandleApproveTimeStep(context, messageData.Chat, messageData.UserInput, ct);

                default:
                    return new ScenarioResponse(ScenarioResult.Transition, messageData.Chat.Id)
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

            return new ScenarioResponse(ScenarioResult.Transition, chat.Id)
            {
                Message = "Куда записываемся?",
                Keyboard = Keyboards.secondStep
            };
        }
        private async Task<ScenarioResponse> HandleBaseProcedureStep(ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            if (context.DataHistory.Count > 0)
                userInput = context.DataHistory.Pop();

            if (userInput != Constants.Manicure && userInput != Constants.Pedicure)
                throw new Exception("Что-то пошло не так");

            await Task.Delay(1, ct);

            context.Data["BaseProcedure"] = userInput;

            context.DataHistory.Push(userInput);

            var procedureData = new Dictionary<string, (string Message, ReplyMarkup Keyboard)>
            {
                [Constants.Manicure] = (Message: Constants.ChooseManicure, Keyboard: Keyboards.thirdManicureStep),
                [Constants.Pedicure] = (Message: Constants.ChoosePedicure, Keyboard: Keyboards.thirdPedicureStep),
                //[Constants.Eyebrows] = (Message: "Выберите форму бровей", Keyboard: Keyboards.eyebrowsStep),
                //[Constants.Lashes] = (Message: "Выберите вид ресничек", Keyboard: Keyboards.lashesStep)
            };

            if (!procedureData.TryGetValue(userInput, out var data))
                throw new Exception("Неизвестная процедура");

            var (message, keyboard) = data;

            context.CurrentStep = "TypeProcedure";

            return new ScenarioResponse(ScenarioResult.Transition, chat.Id)
            {
                Message = message,
                Keyboard = keyboard
            };
        }
        private async Task<ScenarioResponse> HandleTypeProcedureStep(ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            if (context.DataHistory.Count > 1)
                userInput = context.DataHistory.Pop();

            context.Data.TryGetValue("BaseProcedure", out var procedureType);

            if (procedureType == null)
                throw new Exception("Что-то пошло не так");

            context.DataHistory.Push(userInput);

            context.Data["TypeProcedure"] = ProcedureFactory.CreateProcedure(userInput, (string)procedureType);

            var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

            context.CurrentStep = "DateProcedure";

            return new ScenarioResponse(ScenarioResult.Transition, chat.Id)
            {
                Messages = new List<string>() { "Выберите дату", "✖ - означает, что на выбранную дату нет свободных слотов" },
                Keyboards = new List<ReplyMarkup>() { Keyboards.cancelOrBack, Keyboards.DaySlotsKeyboard(DateTime.Today, unavailableSlots)}
            };
        }
        private async Task<ScenarioResponse> HandleChooseDateStep(ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            var date = Helper.ParseDateFromString(userInput);

            context.DataHistory.Push(date.ToString());

            await Task.Delay(1, ct);

            context.Data["DateProcedure"] = date;

            context.CurrentStep = "ApproveDateProcedure";

            return new ScenarioResponse(ScenarioResult.Transition, chat.Id)
            {
                Message = $"Выбранная дата - {date}\n\nВерно?",
                Keyboard = Keyboards.approveDate
            };
        }
        private async Task<ScenarioResponse> HandleApproveDateStep(ScenarioContext context, Chat chat, CancellationToken ct)
        {
            if (!context.Data.TryGetValue("DateProcedure", out var dateObj))
                throw new KeyNotFoundException("Не найден контекст даты");

            if (dateObj is not DateOnly date)
                throw new InvalidCastException($"Ожидался DateOnly, получен {dateObj?.GetType().Name ?? "null"}");

            if (!context.Data.TryGetValue("TimeProcedure", out var message))
                message = "Выберите время";
            else
                message = "Выберите новое время";

            var slots = await _slotService.GetSlotsByDate(date, ct);

            context.CurrentStep = "TimeProcedure";

            return new ScenarioResponse(ScenarioResult.Transition, chat.Id)
            {
                Message = message.ToString(),
                Keyboard = Keyboards.TimeSlotsKeyboard(slots)
            };
        }
        private async Task<ScenarioResponse> HandleChooseTimeStep(ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            if (!TimeOnly.TryParse(userInput, out var time))
                throw new InvalidCastException($"Ожидался TimeOnly, получен {time.GetType().Name ?? "null"}");

            await Task.Delay(1, ct);

            context.DataHistory.Push(time.ToString());

            context.Data["TimeProcedure"] = time;

            context.CurrentStep = "ApproveTimeProcedure";

            return new ScenarioResponse(ScenarioResult.Transition, chat.Id)
            {
                Message = $"Выбранное время - {time}\n\nВерно?",
                Keyboard = Keyboards.approveTime
            };
        }
        private async Task<ScenarioResponse> HandleApproveTimeStep(ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            await _procedureRepository.Add((IProcedure)context.Data["TypeProcedure"], ct);

            var newAppointment = await _appointmentService.AddAppointment(
                (BeautyBotUser)context.Data["User"],
                (IProcedure)context.Data["TypeProcedure"],
                (DateOnly)context.Data["DateProcedure"],
                (TimeOnly)context.Data["TimeProcedure"],
                ct);

            await _slotService.UpdateSlotFromAppointment(newAppointment, ct);

            return new ScenarioResponse(ScenarioResult.Completed, chat.Id)
            {
                Message = $"Вы успешно записаны🤗\n\nЖдём Вас {context.Data["DateProcedure"]} в {context.Data["TimeProcedure"]}\n\nПо адресу г. Екатеринбург ул. Ленина 1, офис 101\n\nПрекрасного дня ☀️",
                Keyboard = Keyboards.firstStep
            };
        }
    
    }
}

