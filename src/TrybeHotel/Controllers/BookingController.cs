using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TrybeHotel.Dto;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("booking")]

    public class BookingController : Controller
    {
        private readonly IBookingRepository _repository;
        public BookingController(IBookingRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        [Authorize(Policy = "Client")]
        public IActionResult Add([FromBody] BookingDtoInsert bookingInsert)
        {
            try
            {
                return StatusCode(201, _repository.Add(bookingInsert, (HttpContext.User.Identity as ClaimsIdentity)!
                .Claims.First(claim => claim.Type == ClaimTypes.Email).Value));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        [HttpGet("{Bookingid}")]
        public IActionResult GetBooking(int Bookingid)
        {
            try
            {
                return Ok(_repository.GetBooking(Bookingid, (HttpContext.User.Identity as ClaimsIdentity)!
                .Claims.First(claim => claim.Type == ClaimTypes.Email).Value));

            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}
