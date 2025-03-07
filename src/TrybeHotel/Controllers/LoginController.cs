using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("login")]

    public class LoginController : Controller
    {

        private readonly IUserRepository _repository;
        public LoginController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDto login)
        {
            try
            {
                var token = new TokenGenerator().Generate(_repository.Login(login));
                return Ok(new { token });
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}
