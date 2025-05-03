using Dapper;
using Microsoft.Data.SqlClient;

namespace ExpenseApi.Data
{
    public class DatabaseInit
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<DatabaseInit> _logger;

        public DatabaseInit(IDbConnectionFactory connectionFactory, ILogger<DatabaseInit> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }


        public async Task InitializeDatabaseAsync()
        {
            var connection = _connectionFactory.CreateConnection();

            if (connection is SqlConnection sqlConnection)
            {
                await sqlConnection.OpenAsync();
            }
            else
            {
                connection.Open();
            }

            _logger.LogInformation("Initializing database...");

            var createExpensesTableSql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Expenses' AND xtype='U')
            CREATE TABLE Expenses (
            Id INT PRIMARY KEY IDENTITY,
            Description NVARCHAR(255) NOT NULL,
            Amount DECIMAL(18, 2) NOT NULL,
            Date DATETIME NOT NULL,
            CategoryId INT NULL,
            UserId UNIQUEIDENTIFIER NOT NULL,
            CONSTRAINT FK_Expenses_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
            CONSTRAINT FK_Expenses_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
            );";

            var createCategoriesTableSql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
            CREATE TABLE Categories (
            Id INT PRIMARY KEY IDENTITY,
            Title NVARCHAR(100) NOT NULL
            );";

            var createUsersTableSql = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
            CREATE TABLE Users (
            UserId UNIQUEIDENTIFIER PRIMARY KEY,
            Name NVARCHAR(100) NULL
            );";

            var insertCategoriesSql = @"
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Title = 'Groceries')
            INSERT INTO Categories (Title) VALUES ('Groceries');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Title = 'Restaurant')
            INSERT INTO Categories (Title) VALUES ('Restaurant');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Title = 'Transport')
            INSERT INTO Categories (Title) VALUES ('Transport');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Title = 'Entertainment')
            INSERT INTO Categories (Title) VALUES ('Entertainment');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Title = 'Utilities')
            INSERT INTO Categories (Title) VALUES ('Utilities');
        IF NOT EXISTS (SELECT 1 FROM Categories WHERE Title = 'Clothing')
            INSERT INTO Categories (Title) VALUES ('Clothing');";


            await connection.ExecuteAsync(createCategoriesTableSql);
            await connection.ExecuteAsync(insertCategoriesSql);
            await connection.ExecuteAsync(createUsersTableSql);
            await connection.ExecuteAsync(createExpensesTableSql);

            connection.Close();
        }

    }
}
