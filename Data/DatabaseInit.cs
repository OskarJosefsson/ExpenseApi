using Dapper;
using Microsoft.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

            var createCategoriesTableSql = @"
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Categories' AND xtype='U')
    CREATE TABLE Categories (
        Id INT PRIMARY KEY IDENTITY,
        Title NVARCHAR(100) NOT NULL
    );";

            var createUsersTableSql = @"
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
    BEGIN
        CREATE TABLE Users (
            UserId UNIQUEIDENTIFIER PRIMARY KEY,
            Name NVARCHAR(100) NULL,
            Email NVARCHAR(256) NULL,
            AvatarUrl NVARCHAR(500) NULL,
            Provider NVARCHAR(100) NOT NULL,
            ProviderUserId NVARCHAR(256) NOT NULL,
            Type NVARCHAR(50) NOT NULL DEFAULT 'Standard',
            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
            LastLogin DATETIME2 NULL
        );
    END";

            var createItemCategoriesTableSql = @"
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ItemCategories' AND xtype='U')
    CREATE TABLE ItemCategories (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(255) NOT NULL
    );";

            var createReceiptItemsTableSql = @"
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ReceiptItems' AND xtype='U')
CREATE TABLE ReceiptItems (
    Name NVARCHAR(255) NOT NULL,
    Cost DECIMAL(18, 2) NOT NULL,
    Amount INT NOT NULL DEFAULT 1,
    ItemCategoryId INT NULL,
    CONSTRAINT FK_ReceiptItems_ItemCategories FOREIGN KEY (ItemCategoryId) REFERENCES ItemCategories(Id)
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

            var insertItemCategoriesSql = @"
    IF NOT EXISTS (SELECT 1 FROM ItemCategories WHERE Name = 'Food')
        INSERT INTO ItemCategories (Name) VALUES ('Food');
    IF NOT EXISTS (SELECT 1 FROM ItemCategories WHERE Name = 'Travel')
        INSERT INTO ItemCategories (Name) VALUES ('Travel');
    IF NOT EXISTS (SELECT 1 FROM ItemCategories WHERE Name = 'Office Supplies')
        INSERT INTO ItemCategories (Name) VALUES ('Office Supplies');";

            var createStoresTableSql = @"
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Stores' AND xtype='U')
    CREATE TABLE Stores (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(255) NOT NULL
    );";

            var createUserReceiptTableSql = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ReceiptUsers' AND xtype='U') CREATE TABLE ReceiptUsers (
    UserId UNIQUEIDENTIFIER NOT NULL, 
    ReceiptId UNIQUEIDENTIFIER NOT NULL,  
    Percentage INT NOT NULL,  
    PRIMARY KEY (UserId, ReceiptId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE, 
    FOREIGN KEY (ReceiptId) REFERENCES Receipts(Id) ON DELETE CASCADE 
);";



            var createReceiptsTableSql = @"
    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Receipts' AND xtype='U')
    CREATE TABLE Receipts (
        Id UNIQUEIDENTIFIER PRIMARY KEY,
        TotalCost DECIMAL(18, 2) NOT NULL,
        StoreId INT NULL,
        CategoryId INT NULL,
        UserId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT FK_Receipts_Stores FOREIGN KEY (StoreId) REFERENCES Stores(Id),
        CONSTRAINT FK_Receipts_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
        CONSTRAINT FK_Receipts_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );";

            var alterReceiptItemsForReceiptsSql = @"
        IF COL_LENGTH('ReceiptItems', 'ReceiptId') IS NULL
    BEGIN
        ALTER TABLE ReceiptItems ADD ReceiptId UNIQUEIDENTIFIER NULL;
        ALTER TABLE ReceiptItems ADD CONSTRAINT FK_ReceiptItems_Receipts FOREIGN KEY (ReceiptId) REFERENCES Receipts(Id);
    END;";

            await connection.ExecuteAsync(createCategoriesTableSql);
            await connection.ExecuteAsync(insertCategoriesSql);
            await connection.ExecuteAsync(createUsersTableSql);
            await connection.ExecuteAsync(createItemCategoriesTableSql);
            await connection.ExecuteAsync(createReceiptItemsTableSql);
            await connection.ExecuteAsync(insertItemCategoriesSql);
            await connection.ExecuteAsync(createStoresTableSql);
            await connection.ExecuteAsync(createReceiptsTableSql);
            await connection.ExecuteAsync(alterReceiptItemsForReceiptsSql);
            await connection.ExecuteAsync(createUserReceiptTableSql);
            await AddStoresAsync();

            connection.Close();
        }


        public async Task AddStoresAsync()
        {

            var csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/TextData/Stores.csv");

            var storeNamesList = File.ReadLines(csvFilePath);

            using var db = _connectionFactory.CreateConnection();

            // Loop through each store name and insert into the database
            foreach (var storeName in storeNamesList)
            {
                var insertStoreSql = @"
            IF NOT EXISTS (SELECT 1 FROM Stores WHERE Name = @StoreName)
                INSERT INTO Stores (Name) VALUES (@StoreName);";

                // Execute the insert query
                await db.ExecuteAsync(insertStoreSql, new { StoreName = storeName });
            }
        }


    }


}
