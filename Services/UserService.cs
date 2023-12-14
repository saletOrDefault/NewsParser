using NewsParser.Models.Users;
using NewsParser.Repositories;
using Microsoft.AspNetCore.Identity;
using NewsParser.Helpers;

namespace NewsParser.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IPasswordHasher<User> passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        this.userRepository = userRepository;
        this.passwordHasher = passwordHasher;
    }

    public async Task Login(UserRequest model)
    {
        var user = await userRepository.Get(model.Login!) ?? throw new MyException("User not found");
        var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            throw new MyException("Password is incorrect");
        }
    }

    public async Task Create(UserRequest model)
    {
        if (await userRepository.Get(model.Login!) != null)
        {
            throw new MyException("User already exists");
        }
        
        var user = new User
        {
            Login = model.Login!,
        };

        var hashedPassword = passwordHasher.HashPassword(user, model.Password);
        user.PasswordHash = hashedPassword;
        
        await userRepository.Create(user);
    }
}