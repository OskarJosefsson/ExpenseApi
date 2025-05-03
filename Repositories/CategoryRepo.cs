using Dapper;
using ExpenseApi.Data;
using ExpenseApi.Models;

namespace ExpenseApi.Services
{
    public interface ICategoryRepo
    {
        Task<Category> CreateCategory(Category category);
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(int id);
        Task<Category?> UpdateCategory(int id, Category category);
        Task<bool> DeleteCategory(int id);
    }
    public class CategoryRepo : ICategoryRepo
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CategoryRepo(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Category> CreateCategory(Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
            INSERT INTO Categories (Title)
            VALUES (@Title);
            SELECT * FROM Categories WHERE Id = SCOPE_IDENTITY();";

            var createdCategory = await connection.QuerySingleOrDefaultAsync<Category>(sql, new { category.Title });
            return createdCategory;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Categories;";
            var categories = await connection.QueryAsync<Category>(sql);
            return categories;
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "SELECT * FROM Categories WHERE Id = @Id;";
            var category = await connection.QuerySingleOrDefaultAsync<Category>(sql, new { Id = id });
            return category;
        }

        public async Task<Category?> UpdateCategory(int id, Category category)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = @"
            UPDATE Categories
            SET Title = @Title
            WHERE Id = @Id;
            SELECT * FROM Categories WHERE Id = @Id;";

            var updatedCategory = await connection.QuerySingleOrDefaultAsync<Category>(sql, new { category.Title, Id = id });
            return updatedCategory;
        }

        public async Task<bool> DeleteCategory(int id)
        {
            using var connection = _connectionFactory.CreateConnection();
            var sql = "DELETE FROM Categories WHERE Id = @Id;";
            var rowsAffected = await connection.ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
    }
}
