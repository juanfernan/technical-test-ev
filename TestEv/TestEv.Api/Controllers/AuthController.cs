using Microsoft.AspNetCore.Mvc;
using TestEv.Application.DTOs;
using TestEv.Application.Interfaces;

namespace TestEv.Api.Controllers
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

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(
            [FromBody] LoginRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
    }
}
