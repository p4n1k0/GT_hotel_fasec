using Microsoft.EntityFrameworkCore;
using TrybeHotel.Models;

namespace TrybeHotel.Repository;
public class TrybeHotelContext : DbContext, ITrybeHotelContext
{
    public DbSet<City> Cities { get; set; } = null!;
    public DbSet<Hotel> Hotels { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;
    public TrybeHotelContext(DbContextOptions<TrybeHotelContext> options) : base(options)
    {
        Seeder.SeedUserAdmin(this);
    }
    public TrybeHotelContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=localhost;Database=TrybeHotel;User=SA;Password=TrybeHotel12!;TrustServerCertificate=True");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) { }

}
