using System.Data;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace ExpenseApi.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            var connString = _configuration.GetConnectionString("DefaultConnection");
            return new SqlConnection(connString);
        }

        public async Task InitializeDatabaseAsync()
        {
            var connection = CreateConnection();

            if (connection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync(); 
            }
            else
            {
                connection.Open(); 
            }

            var tableCheckQuery = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Expenses' AND xtype='U')
            CREATE TABLE Expenses (
                Id INT PRIMARY KEY IDENTITY,
                Description NVARCHAR(255) NOT NULL,
                Amount DECIMAL(18, 2) NOT NULL,
                Date DATETIME NOT NULL
            );";

            await connection.ExecuteAsync(tableCheckQuery);

        }
    }
}
