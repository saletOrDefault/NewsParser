using NewsParser.Models.Users;

namespace NewsParser.Repositories
{
    public interface IUserRepository
    {
        Task<User?> Get(string login);
        Task Create(User user);
    }
}