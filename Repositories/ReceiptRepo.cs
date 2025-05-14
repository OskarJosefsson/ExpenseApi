using Dapper;
using ExpenseApi.Data;
using ExpenseApi.Models;

namespace ExpenseApi.Repositories
{
    public interface IReceiptRepo
    {
        Task<Receipt?> GetByIdAsync(Guid id);
        Task<IEnumerable<Receipt>> GetAllAsync();
        Task CreateAsync(Receipt receipt);
        Task UpdateAsync(Receipt receipt);
        Task DeleteAsync(Guid id);
        Task AddReceiptUserAsync(Guid userId, Guid receiptId, int percentage);
        Task<IEnumerable<ReceiptUsers>> GetReceiptsByUserAsync(Guid userId);

    }

    public class ReceiptRepo : IReceiptRepo
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ReceiptRepo(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<Receipt?> GetByIdAsync(Guid id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"SELECT * FROM Receipts WHERE Id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Receipt>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"SELECT * FROM Receipts";
            return await connection.QueryAsync<Receipt>(sql);
        }

        public async Task CreateAsync(Receipt receipt)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
            INSERT INTO Receipts (Id, TotalCost, StoreId, CategoryId, UserId)
            VALUES (@Id, @TotalCost, @StoreId, @CategoryId, @UserId)";
            await connection.ExecuteAsync(sql, new
            {
                receipt.Id,
                receipt.TotalCost,
                StoreId = receipt.Store?.Id,
                CategoryId = receipt.Category?.Id,
                UserId = receipt.User?.UserId
            });
        }

        public async Task UpdateAsync(Receipt receipt)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
            UPDATE Receipts
            SET TotalCost = @TotalCost,
                StoreId = @StoreId,
                CategoryId = @CategoryId,
                UserId = @UserId
            WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new
            {
                receipt.Id,
                receipt.TotalCost,
                StoreId = receipt.Store?.Id,
                CategoryId = receipt.Category?.Id,
                UserId = receipt.User?.UserId
            });
        }

        public async Task AddReceiptUserAsync(Guid userId, Guid receiptId, int percentage)
        {
            var sql = @"
        IF NOT EXISTS (SELECT 1 FROM ReceiptUsers WHERE UserId = @UserId AND ReceiptId = @ReceiptId)
        INSERT INTO ReceiptUsers (UserId, ReceiptId, Percentage)
        VALUES (@UserId, @ReceiptId, @Percentage)";

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.ExecuteAsync(sql, new { UserId = userId, ReceiptId = receiptId, Percentage = percentage });
            }
        }

        public async Task<IEnumerable<ReceiptUsers>> GetReceiptsByUserAsync(Guid userId)
        {
            var sql = @"
        SELECT * FROM ReceiptUsers
        WHERE UserId = @UserId";

            using (var connection = _connectionFactory.CreateConnection())
            {
                var result = await connection.QueryAsync<ReceiptUsers>(sql, new { UserId = userId });
                return result;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM Receipts WHERE Id = @Id";
            await connection.ExecuteAsync(sql, new { Id = id });
        }
    }

}
