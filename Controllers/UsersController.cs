using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NewsParser.Models.Users;
using NewsParser.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace NewsParser.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService userService;
    private readonly IConfiguration config;

    public UsersController(IUserService userService, IConfiguration config)
    {
        this.userService = userService;
        this.config = config;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserRequest model)
    {
        await userService.Login(model);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var Sectoken = new JwtSecurityToken(config["Jwt:Issuer"],
          config["Jwt:Issuer"],
          null,
          expires: DateTime.Now.AddMinutes(60),
          signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);

        return Ok(token);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserRequest model)
    {
        await userService.Create(model);
        return Ok(new { message = "User created" });
    }
}