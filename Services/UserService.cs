using ExpenseApi.Models;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity.Data;

namespace ExpenseApi.Services
{

    public interface IUserService
    {
        Task<User> GetOrCreateWithGoogle(GoogleJsonWebSignature.Payload payload);
    }
    public class UserService : IUserService
    {

        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }
        public async Task<User> GetOrCreateWithGoogle(GoogleJsonWebSignature.Payload request)
        {

            var existingUser =  await _userRepo.GetByProviderIdAsync("Google", request.Subject);

            if(existingUser != null)
            {

                return existingUser;

            }
            else
            {
                var newUser = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    AvatarUrl = request.Picture,
                    Provider = "Google",
                    ProviderUserId = request.Subject,
                    CreatedAt = DateTime.UtcNow
                };

                return await _userRepo.CreateUser(newUser);
            }
        }
    }
}
