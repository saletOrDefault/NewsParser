namespace NewsParser.Database;

using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using NewsParser.Repositories;

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
        await InitPosts(connection);
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
}