using DMG.Examples.Testing.Domain.Models;
using DMG.Examples.Testing.Services;
using Microsoft.AspNetCore.Mvc;

namespace DMG.Examples.Testing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet(Name = "Get")]
        public async Task<ActionResult<IEnumerable<User>>> Get()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("/{id}",Name = "GetById")]
        public async Task<ActionResult<User>> GetById(string id)
        {
            var user = await _userService.GetAsync(id);
            return Ok(user);
        }

        [HttpPost(Name = "Create")]
        public async Task<ActionResult<User>> Create([FromBody] User user)
        {
            var createdUser = await _userService.CreateAsync(user);
            return CreatedAtAction(
                nameof(GetById), 
                new { id = createdUser.Id }, 
                createdUser
            );
        }
    }
}
