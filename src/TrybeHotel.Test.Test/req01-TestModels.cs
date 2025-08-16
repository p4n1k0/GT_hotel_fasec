namespace trybe_hotel.Test.Test;

using Microsoft.EntityFrameworkCore;
using TrybeHotel.Models;

public class TestReq01
{

    [Trait("TrybeHotel", "1. Adicione o atributo State na model City")]
    [Theory(DisplayName = "City deve conter chave primária")]
    [InlineData("CityId")]
    public void CityShouldContainProperPrimaryKey(string keyName)
    {
        var contextOptions = new DbContextOptionsBuilder<ContextTest>()
            .UseInMemoryDatabase("TrybeHotelContext")
            .Options;
        ContextTest testContext = new(contextOptions);
        DbSet<City> set = testContext.Set<City>();
        var property = set.EntityType.FindProperty(keyName);
        property.IsKey().Should().BeTrue();
    }

    [Trait("TrybeHotel", "1. Adicione o atributo State na model City")]
    [Theory(DisplayName = "City deve conter as propriedades e tipos corretos")]
    [InlineData("CityId", typeof(int))]
    [InlineData("Name", typeof(string))]
    [InlineData("State", typeof(string))]
    [InlineData("Hotels", typeof(IEnumerable<Hotel>))]
    public void CityShouldContainProperties(string propertyName, Type propertyType)
    {
        var propertyToCheck = typeof(City).GetProperty(propertyName);
        propertyToCheck.Should().NotBeNull();
        var propertyTypeName = propertyToCheck.PropertyType;
        propertyTypeName.Should().BeAssignableTo(propertyType);
    }





}