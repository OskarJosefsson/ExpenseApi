using ExpenseApi.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace ExpenseApi.Services
{

    public interface IUserService
    {
        Task<User> GetOrCreate(User request);
    }
    public class UserService : IUserService
    {

        private readonly IUserRepo _userRepo;

        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<User> GetOrCreate(User request)
        {

            var user = await _userRepo.GetByIdAsync(request.UserId);

            if(user != null)
            {
                return user;
            }
            else
            {
                return await _userRepo.CreateUser(request);
            }
        }
    }
}
