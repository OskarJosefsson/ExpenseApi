using Dapper;
using ExpenseApi.Data;
using ExpenseApi.Models;

namespace ExpenseApi.Repositories
{

    public interface IStoreRepo
    {
        Task<Store> CreateAsync(Store store);
        Task<Store?> GetByIdAsync(int id);
        Task<IEnumerable<Store>> GetAllAsync();
        Task<bool> UpdateAsync(Store store);
        Task<bool> DeleteAsync(int id);
    }
    public class StoreRepo : IStoreRepo
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public StoreRepo(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Store> CreateAsync(Store store)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = @"
                INSERT INTO Stores (Name)
                VALUES (@Name);
                SELECT CAST(SCOPE_IDENTITY() AS int);";

            return await db.QuerySingleAsync<Store>(sql, store);
        }

        public async Task<Store?> GetByIdAsync(int id)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Stores WHERE Id = @Id";
            return await db.QuerySingleOrDefaultAsync<Store>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Store>> GetAllAsync()
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Stores";
            return await db.QueryAsync<Store>(sql);
        }

        public async Task<bool> UpdateAsync(Store store)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = @"
                UPDATE Stores
                SET Name = @Name
                WHERE Id = @Id";

            var affected = await db.ExecuteAsync(sql, store);
            return affected > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var db = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Stores WHERE Id = @Id";
            var affected = await db.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}
