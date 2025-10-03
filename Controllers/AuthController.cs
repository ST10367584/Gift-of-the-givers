using Gift_of_the_givers.Models;
using Gift_of_the_givers.Services;


using Microsoft.AspNetCore.Mvc;

namespace Gift_of_the_givers.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            try
            {
                var user = new User
                {
                    Username = userDto.Username,
                    Email = userDto.Email,
                    FullName = userDto.FullName,
                    Role = userDto.Role
                };

                var createdUser = await _authService.Register(user, userDto.Password);
                return Ok(new { message = "User registered successfully", userId = createdUser.UserID });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userDto)
        {
            var user = await _authService.Login(userDto.Username, userDto.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            // In production, generate JWT token here
            return Ok(new
            {
                message = "Login successful",
                userId = user.UserID,
                username = user.Username,
                role = user.Role
            });
        }
    }

    public class UserRegisterDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; } = "Volunteer";
    }

    public class UserLoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}