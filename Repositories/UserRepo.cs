using Dapper;
using ExpenseApi.Data;
using ExpenseApi.Models;

namespace ExpenseApi.Services
{
    public interface IUserRepo
    {
        Task<User> CreateUser(User user);
        Task<User> UpdateUser(Guid userId, User user);
        Task<bool> DeleteUser(Guid userId);
        Task<User?> GetByIdAsync(Guid userId);
    }

    public class UserRepo : IUserRepo
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepo(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<User> CreateUser(User user)
        {
            return null;
        }

        public Task<bool> DeleteUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUser(Guid userId, User user)
        {
            throw new NotImplementedException();
        }
    }
}
