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
        Task<User?> GetByProviderIdAsync(string v, string subject);

        Task<IEnumerable<ReceiptUsers>> GetUsersByReceiptAsync(Guid receiptId);
    }

    public class UserRepo : IUserRepo
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepo(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public Task<bool> DeleteUser(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetByIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetByProviderIdAsync(string provider, string providerUserId)
        {
            const string sql = @"
            SELECT UserId,
                   Name,
                   Email,
                   AvatarUrl,
                   Provider,
                   ProviderUserId,
                   Type,
                   CreatedAt,
                   LastLogin
              FROM Users
             WHERE Provider = @Provider
               AND ProviderUserId = @ProviderUserId;
        ";

            using var conn = _connectionFactory.CreateConnection();
            return await conn.QuerySingleOrDefaultAsync<User>(
                sql,
                new { Provider = provider, ProviderUserId = providerUserId }
            );
        }

        public async Task<IEnumerable<ReceiptUsers>> GetUsersByReceiptAsync(Guid receiptId)
        {
            var sql = @"
        SELECT * FROM ReceiptUsers
        WHERE ReceiptId = @ReceiptId";

            using (var connection = _connectionFactory.CreateConnection())
            {
                var result = await connection.QueryAsync<ReceiptUsers>(sql, new { ReceiptId = receiptId });
                return result;
            }
        }

        public async Task<User> CreateUser(User user)
        {
            const string sql = @"
            INSERT INTO Users (
                UserId,
                Name,
                Email,
                AvatarUrl,
                Provider,
                ProviderUserId,
                Type,
                CreatedAt,
                LastLogin
            ) VALUES (
                @UserId,
                @Name,
                @Email,
                @AvatarUrl,
                @Provider,
                @ProviderUserId,
                @Type,
                @CreatedAt,
                @LastLogin
            );
        ";

            using var conn = _connectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, new
            {
                user.UserId,
                user.Name,
                user.Email,
                user.AvatarUrl,
                user.Provider,
                user.ProviderUserId,
                user.Type,
                user.CreatedAt,
                user.LastLogin
            });

            return user;
        }

        public Task<User> UpdateUser(Guid userId, User user)
        {
            throw new NotImplementedException();
        }
    }
}
