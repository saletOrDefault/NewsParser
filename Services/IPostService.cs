namespace NewsParser.Services;

using NewsParser.Models.Posts;

public interface IPostService
{
    Task<IEnumerable<Post>> GetAll(DateTime? dateFrom, DateTime? dateTo);
    Task<IEnumerable<Post>> Search(string text);
}