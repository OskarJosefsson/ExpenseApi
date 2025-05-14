using Dapper;
using ExpenseApi.Data;
using ExpenseApi.Models;

namespace ExpenseApi.Repositories
{

    public interface IItemCategoryRepo
    {
        Task<int> CreateAsync(ItemCategory category);
        Task<ItemCategory?> GetByIdAsync(int id);
        Task<IEnumerable<ItemCategory>> GetAllAsync();
        Task<bool> UpdateAsync(ItemCategory category);
        Task<bool> DeleteAsync(int id);
    }

    public class ItemCategoryRepo : IItemCategoryRepo
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public ItemCategoryRepo(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAsync(ItemCategory category)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO ItemCategories (Name)
                VALUES (@Name);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await db.QuerySingleAsync<int>(sql, category);
        }

        public async Task<ItemCategory?> GetByIdAsync(int id)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM ItemCategories WHERE Id = @Id";
            return await db.QuerySingleOrDefaultAsync<ItemCategory>(sql, new { Id = id });
        }

        public async Task<IEnumerable<ItemCategory>> GetAllAsync()
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM ItemCategories";
            return await db.QueryAsync<ItemCategory>(sql);
        }

        public async Task<bool> UpdateAsync(ItemCategory category)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = @"
                UPDATE ItemCategories
                SET Name = @Name
                WHERE Id = @Id";

            var affected = await db.ExecuteAsync(sql, category);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM ItemCategories WHERE Id = @Id";
            var affected = await db.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}
