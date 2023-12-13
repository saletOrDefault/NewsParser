using NewsParser.Repositories;
using NewsParser.Models.Posts;

namespace NewsParser.Services;

public class PostService : IPostService
{
    private readonly IPostRepository postRepository;

    public PostService(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task<IEnumerable<Post>> GetAll(DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        return await postRepository.GetAll(dateFrom: dateFrom, dateTo: dateTo);
    }

    public async Task<IEnumerable<Post>> Search(string text)
    {
        return await postRepository.Search(text);
    }

    public async Task<IEnumerable<PostTopResponse>> TopTen()
    {
        return await postRepository.TopTen();
    }
}