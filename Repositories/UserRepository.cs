using Dapper;
using NewsParser.Database;
using NewsParser.Models.Users;

namespace NewsParser.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataContext context;

    public UserRepository(DataContext context)
    {
        this.context = context;
    }

    public async Task<User?> Get(string login)
    {
        using var connection = context.CreateConnection();
        var sql = @"
            SELECT * FROM Users 
            WHERE Login = @login
        ";
        return await connection.QuerySingleOrDefaultAsync<User>(sql, new { login });
    }

    public async Task Create(User user)
    {
        using var connection = context.CreateConnection();
        var sql = @"
            INSERT INTO Users (Login, PasswordHash)
            VALUES (@Login, @PasswordHash)
        ";
        await connection.ExecuteAsync(sql, user);
    }
}