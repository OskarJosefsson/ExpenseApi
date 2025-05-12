using Microsoft.SqlServer.Server;
using Google.Apis.Auth;
using ExpenseApi.Models;
using Newtonsoft.Json.Linq;
namespace ExpenseApi.Services
{
    public interface IAuthService
    {
        Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string idToken);
    }
    public class AuthService :IAuthService
    {
        private readonly GoogleSettings _settings;

        public AuthService(GoogleSettings settings)
        {
            _settings = settings;
        }
        public async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleTokenAsync(string token)
        {
            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _settings.Key }
            };

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(token, validationSettings);
                return payload;
            }
            catch (InvalidJwtException ex)
            {
                return null;
            }
        }
    }
}
