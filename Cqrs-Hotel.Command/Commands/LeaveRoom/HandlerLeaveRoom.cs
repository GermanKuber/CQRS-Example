﻿using System.Threading;
using System.Threading.Tasks;
using Cqrs.Hotel.Data;
using Cqrs.Hotel.Data.Repositories.Interfaces;
using Cqrs.Hotel.Infraestructure.Commands;

namespace Cqrs.Hotel.Command.Commands.LeaveRoom
{
    public class HandlerLeaveRoom : DomainCommandHandler<LeaveRoomCommand, bool>
    {
        private readonly IBookingRepository _bookingRepository;

        public HandlerLeaveRoom(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public override Task<bool> Handle(LeaveRoomCommand request, CancellationToken cancellationToken = default(CancellationToken))
        {
            var booking = _bookingRepository.GetById(request.BookingId);
            booking.Leave();

            _bookingRepository.SaveChange(booking);

            return Task.FromResult<bool>(true);
        }
    }
}