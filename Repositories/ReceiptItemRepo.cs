using Dapper;
using ExpenseApi.Data;
using ExpenseApi.Models;

namespace ExpenseApi.Repositories
{

    public interface IReceiptItemRepo
    {
        Task<int> CreateAsync(ReceiptItem item);
        Task<ReceiptItem?> GetByIdAsync(int id);
        Task<IEnumerable<ReceiptItem>> GetAllAsync();
        Task<bool> UpdateAsync(ReceiptItem item);
        Task<bool> DeleteAsync(int id);
    }


    public class ReceiptItemRepo : IReceiptItemRepo
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ReceiptItemRepo(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(ReceiptItem item)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO ReceiptItems (Name, Cost, Amount, ItemCategoryId)
                VALUES (@Name, @Cost, @Amount, @ItemCategoryId);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await db.QuerySingleAsync<int>(sql, item);
        }

        public async Task<ReceiptItem?> GetByIdAsync(int id)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM ReceiptItems WHERE Id = @Id";
            return await db.QuerySingleOrDefaultAsync<ReceiptItem>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ReceiptItem>> GetAllAsync()
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM ReceiptItems";
            return await db.QueryAsync<ReceiptItem>(sql);
        }

        public async Task<bool> UpdateAsync(ReceiptItem item)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = @"
                UPDATE ReceiptItems
                SET Name = @Name,
                    Cost = @Cost,
                    Amount = @Amount,
                    ItemCategoryId = @ItemCategoryId
                WHERE Id = @Id";

            var affected = await db.ExecuteAsync(sql, item);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM ReceiptItems WHERE Id = @Id";
            var affected = await db.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}


