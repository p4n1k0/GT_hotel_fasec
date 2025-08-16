namespace trybe_hotel.Test.Test;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;

public class TestReq12 : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientBookingPost;

    public TestReq12(WebApplicationFactory<Program> factory)
    {
         _clientBookingPost = factory.CreateClient();
    }


    [Trait("Category", "12. Desenvolva o endpoint GET /geo/address")]
    [Theory(DisplayName = "Será validado que é possível filtrar os hotéis pela localidade")]
    [InlineData("/geo/address")]
    public async Task TestGeoLocation(string url)
    {
        var contextOptions = new DbContextOptionsBuilder<ContextTest>()
            .UseInMemoryDatabase("TrybeHotelContextGeo")
            .Options;
        ContextTest appContext = new(contextOptions);
        appContext.Database.EnsureCreated();
        appContext.Database.EnsureDeleted();
        appContext.Database.EnsureCreated();
        appContext.Cities.Add(new City {CityId = 1, Name = "Manaus", State = "AM" });
        appContext.SaveChanges();
        appContext.Hotels.Add(new Hotel {HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1});
        appContext.SaveChanges();
        appContext.Rooms.Add(new Room { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", HotelId = 1 });
        appContext.SaveChanges();
        var mockRepository = new HotelRepository(appContext);

        var json = "[{\"lat\" : \"-3\", \"lon\":\"-60\"}]";
        var jsonb = "[{\"lat\" : \"-3\", \"lon\":\"-40\"}]";
        var mockClient = new MockHttpMessageHandler();
        mockClient.When($"https://nominatim.openstreetmap.org/*").WithQueryString("street", "Address 1").Respond("application/json",json );
        mockClient.When($"https://nominatim.openstreetmap.org/*").WithQueryString("street", "endereco").Respond("application/json",jsonb );
        var client = new HttpClient(mockClient);

        GeoDto address = new GeoDto {
            Address = "endereco",
            City = "Manaus",
            State = "AM"
        };

        var geoService = new GeoService(client);
        List<GeoDtoHotelResponse> result = await geoService.GetHotelsByGeo(address, mockRepository);
        Assert.Contains("Manaus", result[0].CityName);
        Assert.Equal(1, result[0].HotelId);
        Assert.Equal(2221, result[0].Distance);

    }

}