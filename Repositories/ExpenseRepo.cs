using Dapper;
using ExpenseApi.Data;
using ExpenseApi.Models;
using ExpenseApi.Services;
using System.Data;

    public interface IExpenseRepo
{
    Task<IEnumerable<Expense>> GetAllAsync();

    Task<IEnumerable<Expense>> GetAllByUserAsync();
    Task<Expense?> GetByIdAsync(int id);
    Task<int> CreateAsync(Expense expense);
    Task<bool> UpdateAsync(Expense expense);
    Task<bool> DeleteAsync(int id);
}

public class ExpenseRepo : IExpenseRepo
{
    private readonly IDbConnectionFactory _connectionFactory;

    public ExpenseRepo(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Expense>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Expense>("SELECT * FROM Expenses");
    }

    public async Task<Expense?> GetByIdAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Expense>(
            "SELECT * FROM Expenses WHERE Id = @Id", new { Id = id });
    }

    public async Task<int> CreateAsync(Expense expense)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = @"
        INSERT INTO Expenses (Description, Amount, Date, CategoryId, UserId)
        VALUES (@Description, @Amount, @Date, @CategoryId, @UserId);
        SELECT CAST(SCOPE_IDENTITY() AS int);";

        var parameters = new
        {
            expense.Description,
            expense.Amount,
            expense.Date,
            CategoryId = expense.Category?.Id,
            UserId = expense.User.UserId
        };

        return await connection.ExecuteScalarAsync<int>(sql, parameters);
    }


    public async Task<bool> UpdateAsync(Expense expense)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = @"
            UPDATE Expenses 
            SET Description = @Description, Amount = @Amount, Date = @Date
            WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, expense);
        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var connection = _connectionFactory.CreateConnection();
        var sql = "DELETE FROM Expenses WHERE Id = @Id";
        var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
        return affectedRows > 0;
    }

    public Task<IEnumerable<Expense>> GetAllByUserAsync()
    {
        throw new NotImplementedException();
    }
}
