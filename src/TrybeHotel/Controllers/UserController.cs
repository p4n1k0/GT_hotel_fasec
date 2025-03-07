using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using Microsoft.AspNetCore.Authorization;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("user")]

    public class UserController : Controller
    {
        private readonly IUserRepository _repository;
        public UserController(IUserRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public IActionResult GetUsers()
        {
            return Ok(_repository.GetUsers());
        }

        [HttpPost]
        public IActionResult Add([FromBody] UserDtoInsert user)
        {
            try
            {
                return Created("", _repository.Add(user));
            }
            catch (Exception)
            {
                return Conflict();
            }
        }
    }
}
