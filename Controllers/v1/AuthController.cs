using Ecommerce.API.Dtos.Requests.Auth;
using Ecommerce.API.Dtos.Responses.Auth;
using Ecommerce.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers.v1
{

    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] string request)
        {
            var response = await _authService.RefreshTokenAsync(request);
            return Ok(response);
        }

        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (UserId == null)
                return Unauthorized();
            await _authService.ChangePasswordAsync(UserId, request);
            return Ok(new { message = "Password changed successfully" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            if (UserId != null)
                await _authService.LogoutAsync(UserId);

            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet("profile")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProfile()
        {
            if (UserId == null)
                return Unauthorized();
            var response = await _authService.GetUserProfileAsync(UserId.ToString());
            if (response == null)
                return NotFound();
            return Ok(response);
        }

    }
}