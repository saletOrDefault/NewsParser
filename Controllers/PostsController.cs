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
[Route("api")]
public class PostsController : ControllerBase
{
    private readonly IPostService postService;

    public PostsController(IPostService postService)
    {
        this.postService = postService;
    }

    [HttpGet("posts")]
    public async Task<IActionResult> Posts(DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        var posts = await postService.GetAll(dateFrom: dateFrom, dateTo: dateTo);
        return Ok(posts);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(string text)
    {
        var posts = await postService.Search(text);
        return Ok(posts);
    }
}