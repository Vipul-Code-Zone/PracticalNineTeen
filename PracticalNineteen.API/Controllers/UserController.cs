using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PracticalNineteen.Db.Interfaces;
using PracticalNineteen.Models.ViewModels;

namespace PracticalNineteen.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody]RegisterViewModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _userRepository.RegisterUserAsync(model);
                if(result.IsSuccess)
                {
                    return Ok(result);
                } else
                {
                    return BadRequest(result);
                }
            }
            return BadRequest("Properties are not valid");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userRepository.LoginUserAsync(model);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            return BadRequest("Properties are not valid");
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> LogoutAsync(Logout model)
        {
            if(ModelState.IsValid)
            {
                await _userRepository.LogoutUserAsync(model);
                return Ok();
            }
            return BadRequest();
        }
    }
}
