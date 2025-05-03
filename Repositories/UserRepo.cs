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

        public async Task<User> CreateUser(User user)
        {
            using var connection = _connectionFactory.CreateConnection();

            if (user.UserId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(user.UserId));

            var sql = @"
                INSERT INTO Users (UserId, Name)
                VALUES (@UserId, @Name);
                SELECT * FROM Users WHERE UserId = @UserId;";

            var parameters = new { user.UserId, user.Name };
            var createdUser = await connection.QuerySingleOrDefaultAsync<User>(sql, parameters);

            if (createdUser == null)
                throw new InvalidOperationException("User creation failed.");

            return createdUser;
        }

        public async Task<User?> UpdateUser(Guid userId, User user)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            using var connection = _connectionFactory.CreateConnection();

            var existingUser = await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE UserId = @UserId",
                new { UserId = userId });

            if (existingUser == null)
                return null;

            var sql = @"
                UPDATE Users
                SET Name = @Name
                WHERE UserId = @UserId;
                SELECT * FROM Users WHERE UserId = @UserId;";

            var parameters = new { user.Name, UserId = userId };
            var updatedUser = await connection.QuerySingleOrDefaultAsync<User>(sql, parameters);

            return updatedUser;
        }

        public async Task<bool> DeleteUser(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            using var connection = _connectionFactory.CreateConnection();

            var existingUser = await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE UserId = @UserId",
                new { UserId = userId });

            if (existingUser == null)
                return false;

            var sql = "DELETE FROM Users WHERE UserId = @UserId;";
            var rowsAffected = await connection.ExecuteAsync(sql, new { UserId = userId });

            return rowsAffected > 0;
        }

        public async Task<User?> GetByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            using var connection = _connectionFactory.CreateConnection();

            var sql = "SELECT * FROM Users WHERE UserId = @UserId;";
            var user = await connection.QuerySingleOrDefaultAsync<User>(sql, new { UserId = userId });

            return user;
        }
    }
}
