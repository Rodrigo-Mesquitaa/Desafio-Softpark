using Microsoft.AspNetCore.Mvc;
using Softpark.Application.DTOs;
using Softpark.Application.Services;

namespace Softpark.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Login([FromBody] LoginRequestDto request)
        {
            var response = _authService.Login(request);
            return Ok(response);
        }
    }
}
