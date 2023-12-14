using NewsParser.Models.Users;

namespace NewsParser.Services;

public interface IUserService
{
    Task Login(UserRequest model);
    Task Create(UserRequest model);
}