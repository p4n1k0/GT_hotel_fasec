namespace trybe_hotel.Test.Test;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RichardSzalay.MockHttp;
using System.ComponentModel;

public class TestReq11 : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientGeoStatus;

    public class GeoStatusResponse {
        public string? message { get; set; }
    }
    public TestReq11(WebApplicationFactory<Program> factory)
    {
         _clientGeoStatus = factory.CreateClient();
    }

    [Trait("Category", "11. Desenvolva o endpoint GET /geo/status")]
    [Theory(DisplayName = "Será validado que é obter o status da API")]
    [InlineData("/geo/status")]
    public async Task TestGeoControllerGetResponse(string url)
    {
        var json = "{\"status\" : \"0\", \"message\":\"OK\"}";
        var mockClient = new MockHttpMessageHandler();
        mockClient.When($"https://nominatim.openstreetmap.org/*").Respond("application/json",json );

        var client = new HttpClient(mockClient);
        var geoService = new GeoService(client);
        var result = await geoService.GetGeoStatus();
        GeoStatusResponse jsonResponse = JsonConvert.DeserializeObject<GeoStatusResponse>(result.ToString());
        Assert.Equal("OK", jsonResponse.message);
    }
}