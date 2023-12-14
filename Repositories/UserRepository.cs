using System.Data;
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
        var procedure = "GetUserByLogin";
        using var connection = context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<User>(procedure, new { login }, commandType: CommandType.StoredProcedure);
    }

    public async Task Create(User user)
    {
        var procedure = "AddUser";
        using var connection = context.CreateConnection();
        await connection.ExecuteAsync(procedure, new { user.Login, user.PasswordHash }, commandType: CommandType.StoredProcedure);
    }
}