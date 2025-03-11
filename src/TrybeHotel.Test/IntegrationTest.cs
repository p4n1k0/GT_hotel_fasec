namespace TrybeHotel.Test;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using TrybeHotel.Dto;
using TrybeHotel.Services;
using System.Net.Http.Headers;

public class LoginJson
{
    public string? token { get; set; }
}


public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientTest;

    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDatabase");
                });
                services.AddScoped<ITrybeHotelContext, ContextTest>();
                services.AddScoped<ICityRepository, CityRepository>();
                services.AddScoped<IHotelRepository, HotelRepository>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();
                    appContext.Cities.Add(new City { CityId = 1, Name = "Manaus", State = "AM" });
                    appContext.Cities.Add(new City { CityId = 2, Name = "Palmas", State = "TO" });
                    appContext.SaveChanges();
                    appContext.Hotels.Add(new Hotel { HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1 });
                    appContext.Hotels.Add(new Hotel { HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2 });
                    appContext.Hotels.Add(new Hotel { HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1 });
                    appContext.SaveChanges();
                    appContext.Rooms.Add(new Room { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 2, Name = "Room 2", Capacity = 3, Image = "Image 2", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 3, Name = "Room 3", Capacity = 4, Image = "Image 3", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 4, Name = "Room 4", Capacity = 2, Image = "Image 4", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 5, Name = "Room 5", Capacity = 3, Image = "Image 5", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 6, Name = "Room 6", Capacity = 4, Image = "Image 6", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 7, Name = "Room 7", Capacity = 2, Image = "Image 7", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 8, Name = "Room 8", Capacity = 3, Image = "Image 8", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 9, Name = "Room 9", Capacity = 4, Image = "Image 9", HotelId = 3 });
                    appContext.SaveChanges();
                    appContext.Users.Add(new User { UserId = 1, Name = "Ana", Email = "ana@trybehotel.com", Password = "Senha1", UserType = "admin" });
                    appContext.Users.Add(new User { UserId = 2, Name = "Beatriz", Email = "beatriz@trybehotel.com", Password = "Senha2", UserType = "client" });
                    appContext.Users.Add(new User { UserId = 3, Name = "Laura", Email = "laura@trybehotel.com", Password = "Senha3", UserType = "client" });
                    appContext.SaveChanges();
                    appContext.Bookings.Add(new Booking { BookingId = 1, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 2, RoomId = 1 });
                    appContext.Bookings.Add(new Booking { BookingId = 2, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 3, RoomId = 4 });
                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }

    [Trait("Category", "Testando endpoint GET /city")]
    [Theory(DisplayName = "Será validado se a resposta do status é 200")]
    [InlineData("/city")]
    public async Task TestGetCity(string url)
    {
        Assert.Equal(System.Net.HttpStatusCode.OK, (await _clientTest.GetAsync(url))?.StatusCode);
    }


    [Trait("Category", "Testando endpoint POST /city")]
    [Theory(DisplayName = "Será validado se a resposta do status code é 201 e o JSON de resposta correto")]
    [InlineData("/city")]
    public async Task TestPostCity(string url)
    {
        Assert.Equal(System.Net.HttpStatusCode.Created, (await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new City
        { CityId = 3, Name = "Belém" }), Encoding.UTF8, "application/json"))).StatusCode);
    }


    [Trait("Category", "Testando endpoint GET /hotel")]
    [Theory(DisplayName = "Será validado se a resposta do status code é 200 e o JSON de resposta está correto")]
    [InlineData("/hotel")]
    public async Task TestGetHotel(string url)
    {
        Assert.Equal(System.Net.HttpStatusCode.OK, (await _clientTest.GetAsync(url)).StatusCode);
    }


    [Trait("Category", "Testando endpoint POST /hotel")]
    [Theory(DisplayName = "Será validado se a resposta do status code é 201 e o JSON de resposta está correto")]
    [InlineData("/hotel")]
    public async Task TestPostHotel(string url)
    {
        _clientTest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new TokenGenerator().Generate(new UserDto
        {
            UserId = 1,
            Name = "Ana",
            Email = "ana@trybehotel.com",
            UserType = "admin",
        }));

        Assert.Equal(System.Net.HttpStatusCode.Created, (await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(new Hotel
        {
            Name = "Marrocos",
            Address = "Address 4",
            CityId = 1
        }), Encoding.UTF8, "application/json"))).StatusCode);
    }


    [Trait("Category", "Testando endpoint GET /room/{hotelId}")]
    [Theory(DisplayName = "Será validado se a resposta do status code é 200 e o JSON de resposta está correto")]
    [InlineData("/room/1")]
    public async Task TestGetRoomByHotelIdStatusCodeOk(string url)
    {
        Assert.Equal(System.Net.HttpStatusCode.OK, (await _clientTest.GetAsync(url)).StatusCode);
    }

}
