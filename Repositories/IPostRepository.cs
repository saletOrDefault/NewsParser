using NewsParser.Models.Posts;

namespace NewsParser.Repositories
{
    public interface IPostRepository
    {
        Task<IEnumerable<Post>> GetAll(DateTime? dateFrom, DateTime? dateTo);
        Task<IEnumerable<Post>> Search(string text);
        Task Create(Post post);
        Task<IEnumerable<PostTopResponse>> TopTen();
    }
}