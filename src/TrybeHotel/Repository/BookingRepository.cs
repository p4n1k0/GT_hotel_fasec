using TrybeHotel.Models;
using TrybeHotel.Dto;
using Microsoft.EntityFrameworkCore;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            if (booking.GuestQuant > GetRoomById(booking.RoomId).Capacity)
            {
                throw new BadHttpRequestException("Guest quantity over room capacity");
            }

            var entity = new Booking
            {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                UserId = _context.Users
                .Select(user => new { user.Email, user.UserId })
                .First(user => user.Email == email).UserId,
                RoomId = booking.RoomId,
            };

            _context.Bookings.Add(entity);
            _context.SaveChanges();

            return GetBooking(entity.BookingId, email);
        }

        public BookingResponse GetBooking(int bookingId, string email)
        {
            if (_context.Users.Select(user => new { user.Email, user.UserId})
            .First(user => user.Email == email).UserId != _context.Bookings
            .Select(booking => new { booking.BookingId, booking.UserId})
            .First(booking => booking.BookingId == bookingId)
            .UserId)
            {
                throw new UnauthorizedAccessException();
            }

            return _context.Bookings
            .Include(booking => booking.Room)
            .ThenInclude(room => room!.Hotel)
            .ThenInclude(hotel => hotel!.City)
            .Where(booking => booking.BookingId == bookingId)
            .Select(booking => new BookingResponse
            {
                BookingId = booking.BookingId,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                Room = new RoomDto
                {
                    RoomId = booking.RoomId,
                    Name = booking.Room!.Name,
                    Capacity = booking.Room.Capacity,
                    Image = booking.Room.Image,
                    Hotel = new HotelDto
                    {
                        HotelId = booking.Room.Hotel!.HotelId,
                        Name = booking.Room.Hotel.Name,
                        Address = booking.Room.Hotel.Address,
                        CityId = booking.Room.Hotel.CityId,
                        CityName = booking.Room.Hotel.City!.Name,
                    }
                }
            }).First();
        }

        public Room GetRoomById(int RoomId)
        {
            return _context.Rooms.Include(room => room.Hotel).First(room => room.RoomId == RoomId);
        }

    }

}
