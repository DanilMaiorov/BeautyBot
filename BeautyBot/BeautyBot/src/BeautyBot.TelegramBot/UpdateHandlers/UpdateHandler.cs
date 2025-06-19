using BeautyBot.src.BeautyBot.Core.Interfaces;
using BeautyBot.src.BeautyBot.Domain.Services;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeautyBot.src.BeautyBot.TelegramBot.UpdateHandlers
{
    public class UpdateHandler : Otus.ToDoList.ConsoleBot.IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly IProcedureCatalogService _procedureCatalogService;
        private readonly IPriceCalculationService _priceCalculationService;

        CancellationToken _cancellationToken;

        public UpdateHandler(
            IUserService userService,
            IAppointmentService appointmentService,
            IProcedureCatalogService procedureCatalogService,
            IPriceCalculationService priceCalculationService,
            CancellationToken ct)
        {
            _userService = userService;
            _appointmentService = appointmentService;
            _procedureCatalogService = procedureCatalogService;
            _priceCalculationService = priceCalculationService;
            _cancellationToken = ct;
        }
        public Task HandleUpdateAsync(Otus.ToDoList.ConsoleBot.ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            var currentChat = update.Message.Chat;
            string message = "";

            return Task.Delay(1);
        }





        public Task HandleErrorAsync(Otus.ToDoList.ConsoleBot.ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            throw new NotImplementedException();
        }



        //public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
        //{
        // ... логика обработки сообщений ...

        //    if (update.Type == UpdateType.Message && update.Message?.Text != null)
        //    {
        //        var message = update.Message;
        //        var user = _userService.GetUser(message.From.Id);
        //        if (user == null)
        //        {
        //            user = _userService.RegisterUser(message.From.Id, message.From.Username ?? message.From.FirstName);
        //            // Отправить приветствие новому пользователю
        //        }

        //        if (message.Text == "/start")
        //        {
        //            // Отправить список доступных процедур
        //            var procedures = _procedureCatalogService.GetAllProcedures();
        //            // ... формирование ответа ...
        //        }
        //        else if (message.Text.StartsWith("/book"))
        //        {
        //            // Предположим, парсим ID процедуры из сообщения
        //            if (Guid.TryParse(message.Text.Replace("/book ", ""), out Guid procedureId))
        //            {
        //                var procedure = _procedureCatalogService.GetProcedureById(procedureId);
        //                if (procedure != null)
        //                {
        //                    // Пример: бронирование на текущее время (упрощенно)
        //                    var appointment = _appointmentService.BookAppointment(user.Id, procedure.Id, DateTime.Now.AddHours(1));

        //                    // Пример использования PriceCalculationService
        //                    var price = _priceCalculationService.CalculatePrice(new ProcedureCostDto(procedure));
        //                    // ... подтверждение бронирования с ценой ...
        //                }
        //            }
        //        }
        //        // ... другие команды ...
        //    }
        //}

        //public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        //{
        //    // ... логика обработки ошибок ...
        //}
    }
}
