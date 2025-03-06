using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 4. Refatore o endpoint GET /city
        public IEnumerable<CityDto> GetCities()
        {
            return _context.Cities.Select(city => new CityDto
            {
                CityId = city.CityId,
                Name = city.Name,
                State = city.State,
            }).ToList();
        }

        // 2. Refatore o endpoint POST /city
        public CityDto AddCity(City city)
        {
            _context.Cities.Add(city);
            _context.SaveChanges();

            return new CityDto
            {
                CityId = city.CityId,
                Name = city.Name,
                State = city.State,
            };
        }

        // 3. Desenvolva o endpoint PUT /city
        public CityDto UpdateCity(City city)
        {
            _context.Cities.First(c => c.CityId == city.CityId).State = city.State;
            _context.Cities.First(c => c.CityId == city.CityId).Name = city.Name;
            _context.SaveChanges();

            return new CityDto
            {
                CityId = _context.Cities.First(c => c.CityId == city.CityId).CityId,
                Name = _context.Cities.First(c => c.CityId == city.CityId).Name,
                State = _context.Cities.First(c => c.CityId == city.CityId).State,
            };

        }

    }
}
