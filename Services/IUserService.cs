namespace NewsParser.Services;

using NewsParser.Models.Users;

public interface IUserService
{
    Task Login(UserRequest model);
    Task Create(UserRequest model);
}