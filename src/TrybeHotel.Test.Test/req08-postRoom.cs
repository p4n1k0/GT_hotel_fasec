namespace trybe_hotel.Test.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;



public class RoomPostJson {
        public int RoomId { get; set; }
        public string? Name { get; set; }
        public int Capacity { get; set; }
        public string? Image { get; set; }
        public HotelJson? Hotel { get; set; }
}

public class TestReq08 : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientRoomPost;

    public TestReq08(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientRoomPost = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestPostRoom");
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
                    appContext.Cities.Add(new City {CityId = 1, Name = "Manaus", State = "AM"});
                    appContext.Cities.Add(new City {CityId = 2, Name = "Palmas", State = "TO"});
                    appContext.SaveChanges();
                    appContext.Hotels.Add(new Hotel {HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1});
                    appContext.Hotels.Add(new Hotel {HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2});
                    appContext.Hotels.Add(new Hotel {HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Address 3", CityId = 1});
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
                    appContext.Bookings.Add(new Booking { BookingId = 1, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 2, RoomId = 1});
                    appContext.Bookings.Add(new Booking { BookingId = 2, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 3, RoomId = 4});
                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }
   

    [Trait("Category", "8. Refatore o endpoint POST /room")]
    [Theory(DisplayName = "Será validado que é possível realizar a operação com sucesso")]
    [InlineData("/room")]
    public async Task TestHotelControllerPostResponse(string url)
    {
        var inputLogin = new {
            Email = "ana@trybehotel.com",
            Password = "Senha1"
        };
        var responseLogin = await _clientRoomPost.PostAsync("/login",new StringContent(JsonConvert.SerializeObject(inputLogin), System.Text.Encoding.UTF8, "application/json"));
        var responseLoginString = await responseLogin.Content.ReadAsStringAsync();
        LoginJson jsonLogin = JsonConvert.DeserializeObject<LoginJson>(responseLoginString);

        var inputObj = new {
            Name = "Suite básica",
            Capacity = 8,
            Image = "Image suite",
            HotelId = 3
        };
        
        _clientRoomPost.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jsonLogin.token);

        var response = await _clientRoomPost.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        RoomPostJson jsonResponse = JsonConvert.DeserializeObject<RoomPostJson>(responseString);
        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
        Assert.Equal(10, jsonResponse.RoomId);
        Assert.Contains("Suite básica", jsonResponse.Name);
        Assert.Equal(8, jsonResponse.Capacity);
        Assert.Contains("Image suite", jsonResponse.Image);
        Assert.Contains("Trybe Hotel Ponta Negra", jsonResponse.Hotel.Name);
        Assert.Contains("Manaus", jsonResponse.Hotel.CityName);
        Assert.Contains("AM", jsonResponse.Hotel.State);
    }

    [Trait("Category", "8. Refatore o endpoint POST /room")]
    [Theory(DisplayName = "Será validado que o status será proibido caso o acesso não seja admin")]
    [InlineData("/room")]
    public async Task TestHotelControllerPostResponseForbidden(string url)
    {
         var inputLogin = new {
            Email = "beatriz@trybehotel.com",
            Password = "Senha2"
        };
        var responseLogin = await _clientRoomPost.PostAsync("/login",new StringContent(JsonConvert.SerializeObject(inputLogin), System.Text.Encoding.UTF8, "application/json"));
        var responseLoginString = await responseLogin.Content.ReadAsStringAsync();
        LoginJson jsonLogin = JsonConvert.DeserializeObject<LoginJson>(responseLoginString);

        var inputObj = new {
            Name = "Suite básica",
            Capacity = 8,
            Image = "Image suite",
            HotelId = 3
        };
        
        _clientRoomPost.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jsonLogin.token);

        var response = await _clientRoomPost.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Forbidden, response?.StatusCode);
    }

    
    [Trait("Category", "5. Adicione a autorização de admin no endpoint /POST room")]
    [Theory(DisplayName = "Será validado que o status será não autorizado caso o acesso não exista")]
    [InlineData("/room")]
    public async Task TestHotelControllerPostResponseUnathorized(string url)
    {
        var inputObj = new {
            Name = "Suite básica",
            Capacity = 8,
            Image = "Image suite",
            HotelId = 3
        };
        
        var response = await _clientRoomPost.PostAsync(url,new StringContent(JsonConvert.SerializeObject(inputObj), System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response?.StatusCode);
    }
}