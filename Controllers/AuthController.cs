using ExpenseApi.Models;
using ExpenseApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpenseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public AuthController(JwtSettings jwtSettings, IUserService userService, IAuthService authService)
        {
            _jwtSettings = jwtSettings;
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User model)
        {
            return Unauthorized();
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            var payload = await _authService.VerifyGoogleTokenAsync(request.Token);
            if (payload == null)
                return Unauthorized();

            // Find or create user using payload info
            var user = await _userService.GetOrCreateWithGoogle(payload);

            if (user == null)
                return Unauthorized();

            if(user != null && !string.IsNullOrEmpty(user.Email))
            {
                var jwt = GenerateJwtToken(user.Email);
                return Ok(new { token = jwt });
            }

            return Unauthorized();
        }

        private string GenerateJwtToken(string username)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            try
            {
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }


        }
    }
}
