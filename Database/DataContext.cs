using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace NewsParser.Database;

public class DataContext
{
    private readonly DbSettings dbSettings;

    public DataContext(IOptions<DbSettings> dbSettings)
    {
        this.dbSettings = dbSettings.Value;
    }

    public IDbConnection CreateConnection()
    {
        var connectionString = $"Server={dbSettings.Server};Database={dbSettings.Database};User={dbSettings.User};Password={dbSettings.Password};Trust Server Certificate=True;";
        return new SqlConnection(connectionString);
    }

    public async Task Init()
    {
        await InitDatabase();
        await InitTables();
    }

    private async Task InitDatabase()
    {
        // create database if it doesn't exist
        var connectionString = $"Server={dbSettings.Server};Database=master;User={dbSettings.User};Password={dbSettings.Password};Trust Server Certificate=True;";
        using var connection = new SqlConnection(connectionString);
        var sql = $"IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '{dbSettings.Database}') CREATE DATABASE [{dbSettings.Database}];";
        await connection.ExecuteAsync(sql);
    }

    private async Task InitTables()
    {
        // create tables if they don't exist
        using var connection = CreateConnection();
        await InitUsers(connection);
        await InitUsersProcedures(connection);
        await InitPosts(connection);
        await InitPostsProcedures(connection);
    }

    private async Task InitUsers(IDbConnection connection)
    {
        var sql = @"
                IF OBJECT_ID('Users', 'U') IS NULL
                CREATE TABLE Users (
                    Id INT NOT NULL PRIMARY KEY IDENTITY,
                    Login NVARCHAR(MAX),
                    PasswordHash NVARCHAR(MAX),
                    PasswordSalt NVARCHAR(MAX)
                );
            ";
        await connection.ExecuteAsync(sql);
    }

    private async Task InitUsersProcedures(IDbConnection connection)
    {
        var sql = @"
                CREATE OR ALTER PROCEDURE GetUserByLogin
                    (@login NVARCHAR(MAX))  
                AS  
                BEGIN    
                    SELECT * FROM Users WHERE Login = @login  
                END 
            ";
        await connection.ExecuteAsync(sql);

        sql = @"
                CREATE OR ALTER PROCEDURE AddUser
                    (@login NVARCHAR(MAX),
                    @passwordHash NVARCHAR(MAX))  
                AS  
                BEGIN    
                    INSERT INTO Users (Login, PasswordHash)
                    VALUES (@login, @passwordHash)
                END 
            ";
        await connection.ExecuteAsync(sql);
    }

    private async Task InitPosts(IDbConnection connection)
    {
        var sql = @"
                IF OBJECT_ID('Posts') IS NULL
                CREATE TABLE Posts (
                    Id INT NOT NULL PRIMARY KEY IDENTITY,
                    Title NVARCHAR(500),
                    Content NVARCHAR(MAX),
                    PostedDate DATE,
                    CleanedContent NVARCHAR(MAX),
                    CONSTRAINT uniquePost UNIQUE(Title, PostedDate)
                );
            ";
        await connection.ExecuteAsync(sql);
    }

    private async Task InitPostsProcedures(IDbConnection connection)
    {
        var sql = @"
                CREATE OR ALTER PROCEDURE SearchPosts
                    (@search NVARCHAR(MAX))
                AS  
                BEGIN    
                    SELECT * FROM Posts WHERE Content LIKE @search
                END 
            ";
        await connection.ExecuteAsync(sql);

        sql = @"
                CREATE OR ALTER PROCEDURE AddPost
                    (@title NVARCHAR(MAX),
                    @content NVARCHAR(MAX),
                    @postedDate DATE,
                    @cleanedContent NVARCHAR(MAX))  
                AS  
                BEGIN    
                    INSERT INTO Posts (Title, Content, PostedDate, CleanedContent)
                    VALUES (@title, @content, @postedDate, @cleanedContent)
                END 
            ";
        await connection.ExecuteAsync(sql);

        sql = @"
                CREATE OR ALTER PROCEDURE TopTen
                AS  
                BEGIN    
                    SELECT TOP 10 value as word, COUNT(*) AS [NumberOfOccurence]
                    FROM Posts
                    CROSS APPLY STRING_SPLIT(CleanedContent, ' ')
                    GROUP BY value
                    HAVING LEN(value) > 0
                    ORDER BY COUNT(*) DESC
                END 
            ";
        await connection.ExecuteAsync(sql);
    }
}