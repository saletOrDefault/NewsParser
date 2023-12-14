using NewsParser.Models.Posts;

namespace NewsParser.Services;

public interface IPostService
{
    Task<IEnumerable<Post>> GetAll(DateTime? dateFrom, DateTime? dateTo);
    Task<IEnumerable<Post>> Search(string text);
    Task<IEnumerable<PostTopResponse>> TopTen();
}