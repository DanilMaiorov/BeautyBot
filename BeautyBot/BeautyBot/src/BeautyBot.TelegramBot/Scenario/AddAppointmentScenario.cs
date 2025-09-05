using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Entities;
using BeautyBot.src.BeautyBot.Domain.Services;
using BeautyBot.src.BeautyBot.Infrastructure.Repositories.InMemory;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeautyBot.src.BeautyBot.TelegramBot.Scenario
{
    public class AddAppointmentScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly ISlotService _slotService;

        private readonly PostgreSqlProcedureRepository _procedureRepository;

        public AddAppointmentScenario(IUserService userService, IAppointmentService appointmentService, ISlotService slotService, PostgreSqlProcedureRepository procedureRepository)
        {
            _userService = userService;
            _appointmentService = appointmentService;

            _slotService = slotService;

            _procedureRepository = procedureRepository;
        }

        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddAppointment;
        }

        public async Task<ScenarioResponse> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Update update, CancellationToken ct)
        {
            //верну выполненный сценарий если придёт какая-то левая инфа
            //if (update.Message == null && update.CallbackQuery == null)
            //    return ScenarioResult.Completed;
            if (update.Message == null && update.CallbackQuery == null)
                return new ScenarioResponse() 
                    {
                        Result = ScenarioResult.Completed
                    };

            (Chat? currentChat, string? currentUserInput, int currentMessageId, BeautyBotUser? currentUser) = await Helper.HandleMessageAsyncGetData(update, _userService, ct);

            switch (context.CurrentStep)
            {
                case null:
                    return await HandleInitialStep(botClient, context, currentUser, currentChat, ct);

                case "BaseProcedure":
                    return await HandleBaseProcedureStep(botClient, context, currentChat, currentUserInput, ct);

                case "TypeProcedure":
                    return await HandleTypeProcedureStep(botClient, context, currentChat, currentUserInput, currentMessageId, ct);

                case "DateProcedure":
                    return await HandleChooseDateStep(botClient, context, currentChat, currentUserInput, currentMessageId, ct);

                case "ApproveDateProcedure":
                    return await HandleApproveDateStep(botClient, context, currentChat, currentUserInput, currentMessageId, ct);

                case "TimeProcedure":
                    return await HandleChooseTimeStep(botClient, context, currentChat, currentUserInput, ct);

                case "ApproveTimeProcedure":
                    return await HandleApproveTimeStep(botClient, context, currentChat, currentUserInput, ct);
                default:
                    await botClient.SendMessage(currentChat, "Неизвестный шаг сценария", replyMarkup: Keyboards.firstStep, cancellationToken: ct);
                    break;
            }
            return new ScenarioResponse()
                {
                    Result = ScenarioResult.Completed
                };
        }
        private async Task<ScenarioResponse> HandleInitialStep(ITelegramBotClient botClient, ScenarioContext context, BeautyBotUser user, Chat chat, CancellationToken ct)
        {
            context.Data["User"] = user;

            context.CurrentStep = "BaseProcedure";

            return new ScenarioResponse()
            {
                Result = ScenarioResult.Transition,
                Message = "Куда записываемся?",
                Chat = chat.Id,
                Keyboard = Keyboards.secondStep
            };
        }
        private async Task<ScenarioResponse> HandleBaseProcedureStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            if (userInput == Constants.Back)
            {
                context.DataHistory.Pop();

                context.CurrentStep = null;

                return await HandleInitialStep(botClient, context, (BeautyBotUser)context.Data["User"], chat, ct);
            }

            if (userInput != Constants.Manicure && userInput != Constants.Pedicure)
                throw new Exception("Что-то пошло не так");

            context.Data["BaseProcedure"] = userInput;

            context.DataHistory.Push(userInput);

            var procedureData = new Dictionary<string, (string Message, ReplyMarkup Keyboard)>
            {
                [Constants.Manicure] = (Message: "Выберите маникюр", Keyboard: Keyboards.thirdManicureStep),
                [Constants.Pedicure] = (Message: "Выберите педикюр", Keyboard: Keyboards.thirdPedicureStep),
                //[Constants.Eyebrows] = (Message: "Выберите форму бровей", Keyboard: Keyboards.eyebrowsStep),
                //[Constants.Lashes] = (Message: "Выберите вид ресничек", Keyboard: Keyboards.lashesStep)
            };

            if (!procedureData.TryGetValue(userInput, out var data))
                throw new Exception("Неизвестная процедура");

            var (message, keyboard) = data;

            context.CurrentStep = "TypeProcedure";

            return new ScenarioResponse()
                {
                    Result = ScenarioResult.Transition,
                    Message = message,
                    Chat = chat.Id,
                    Keyboard = keyboard
                };
        }
        private async Task<ScenarioResponse> HandleTypeProcedureStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, int messageId, CancellationToken ct)
        {
            context.Data.TryGetValue("BaseProcedure", out var procedureType);

            if (procedureType == null)
                throw new Exception("Что-то пошло не так");

            if (userInput == Constants.Back)
            {
                context.DataHistory.Pop();

                context.CurrentStep = "BaseProcedure";

                //решить что-то с удалением
                await botClient.DeleteMessage(chatId: chat, messageId: messageId - 1, cancellationToken: ct);

                return await HandleBaseProcedureStep(botClient, context, chat, context.DataHistory.Peek(), ct);
            }

            context.DataHistory.Push(userInput);

            context.Data["TypeProcedure"] = ProcedureFactory.CreateProcedure(userInput, (string)procedureType);

            var unavailableSlots = await _slotService.GetUnavailableSlotsByDate(ct);

            context.CurrentStep = "DateProcedure";

            return new ScenarioResponse()
                {
                    Result = ScenarioResult.Transition,
                    Messages = new List<string>() { "Выберите дату", "✖ - означает, что на выбранную дату нет свободных слотов" },
                    Chat = chat.Id,
                    Keyboards = new List<ReplyMarkup>() { Keyboards.cancelOrBack, Keyboards.DaySlotsKeyboard(unavailableSlots)}
                };
        }
        private async Task<ScenarioResponse> HandleChooseDateStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, int messageId, CancellationToken ct)
        {
            if (userInput == Constants.Back)
            {
                context.CurrentStep = "TypeProcedure";

                return await HandleTypeProcedureStep(botClient, context, chat, context.DataHistory.Pop(), messageId, ct);
            }

            var date = Helper.ParseDateFromString(userInput);

            context.Data["DateProcedure"] = date;

            context.CurrentStep = "ApproveDateProcedure";

            return new ScenarioResponse()
                {
                    Result = ScenarioResult.Transition,
                    Message = $"Выбранная дата - {date}\n\nВерно?",
                    Chat = chat.Id,
                    Keyboard = Keyboards.approveDate
                };
        }
        private async Task<ScenarioResponse> HandleApproveDateStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, int messageId, CancellationToken ct)
        {
            if (!context.Data.TryGetValue("DateProcedure", out var dateObj))
                throw new KeyNotFoundException("Не найдена контекст даты");

            if (dateObj is not DateOnly date)
                throw new InvalidCastException($"Ожидался DateOnly, получен {dateObj?.GetType().Name ?? "null"}");

            var slots = await _slotService.GetSlotsByDate(date, ct);

            context.CurrentStep = "ChooseTimeProcedure";

            return new ScenarioResponse()
                {
                    Result = ScenarioResult.Transition,
                    Message = "Выберите время",
                    Chat = chat.Id,
                    Keyboard = Keyboards.TimeSlotsKeyboard(slots)
                };
        }
        private async Task<ScenarioResponse> HandleChooseTimeStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            if (!TimeOnly.TryParse(userInput, out var time))
                throw new InvalidCastException($"Ожидался TimeOnly, получен {time.GetType().Name ?? "null"}");

            context.Data["TimeProcedure"] = time;

            context.CurrentStep = "ApproveTimeProcedure";

            return new ScenarioResponse()
                {
                    Result = ScenarioResult.Transition,
                    Message = $"Выбранное время - {time}\n\nВерно?",
                    Chat = chat.Id,
                    Keyboard = Keyboards.approveTime
                };
        }

        private async Task<ScenarioResponse> HandleApproveTimeStep(ITelegramBotClient botClient, ScenarioContext context, Chat chat, string userInput, CancellationToken ct)
        {
            //if (userInput != Constants.Accept)
            //{
            //    if (!context.Data.TryGetValue("DateProcedure", out var dateObj))
            //        throw new KeyNotFoundException("Не найдена контекст даты");

            //    if (dateObj is not DateOnly date)
            //        throw new InvalidCastException($"Ожидался DateOnly, получен {dateObj?.GetType().Name ?? "null"}");

            //    var slots = await _slotService.GetSlotsByDate(date, ct);

            //    context.Data["Time"] = null;

            //    context.CurrentStep = "ChooseTimeProcedure";

            //    return new ScenarioResponse()
            //        {
            //            Result = ScenarioResult.Transition,
            //            Message = "Выберите другое время",
            //            Chat = chat.Id,
            //            Keyboard = Keyboards.TimeSlotsKeyboard(slots)
            //        };
            //}

            await _procedureRepository.Add((IProcedure)context.Data["TypeProcedure"], ct);

            var newAppointment = await _appointmentService.AddAppointment(
                (BeautyBotUser)context.Data["User"],
                (IProcedure)context.Data["TypeProcedure"],
                (DateOnly)context.Data["DateProcedure"],
                (TimeOnly)context.Data["TimeProcedure"],
                ct);

            await _slotService.UpdateSlotFromAppointment(newAppointment, ct);

            return new ScenarioResponse()
            {
                Result = ScenarioResult.Completed,
                Message = $"Вы успешно записаны🤗\n\nЖдём Вас {context.Data["DateProcedure"]} в {context.Data["TimeProcedure"]}\n\nПо адресу г. Екатеринбург ул. Ленина 1, офис 101\n\nПрекрасного дня ☀️",
                Chat = chat.Id,
                Keyboard = Keyboards.firstStep
            };
        }


















    }
}

