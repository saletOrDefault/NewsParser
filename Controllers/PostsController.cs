using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsParser.Services;

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

    [HttpGet("topten")]
    public async Task<IActionResult> TopTen()
    {
        var posts = await postService.TopTen();
        return Ok(posts);
    }
}