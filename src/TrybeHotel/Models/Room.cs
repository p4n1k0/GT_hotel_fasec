namespace TrybeHotel.Models;
using System.ComponentModel.DataAnnotations;

// 1. Implemente as models da aplicação
public class Room {
    [Key]
    public int RoomId { get; set; }
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public string? Image { get; set; }
    public int HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    public ICollection<Booking>? Bookings { get; set; }
}
