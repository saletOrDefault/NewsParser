using System.Text;
using Dapper;
using NewsParser.Database;
using NewsParser.Models.Posts;

namespace NewsParser.Repositories;

public class PostRepository : IPostRepository
{
    private readonly DataContext context;

    public PostRepository(DataContext context)
    {
        this.context = context;
    }

    public async Task<IEnumerable<Post>> GetAll(DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        using var connection = context.CreateConnection();
        var sql = new StringBuilder(@"SELECT * FROM Posts");
        if (dateFrom != null)
        {
            sql.Append(" WHERE PostedDate >= @dateFrom");
            if (dateTo != null)
            {
                sql.Append(" AND PostedDate <= @dateTo");
            }
        }
        else if (dateTo != null)
        {
            sql.Append(" WHERE PostedDate <= @dateTo");
        }

        return await connection.QueryAsync<Post>(sql.ToString(), new { dateFrom, dateTo });
    }

    public async Task<IEnumerable<Post>> Search(string text)
    {
        using var connection = context.CreateConnection();
        var search = "%" + text.Replace("[", "[[]").Replace("%", "[%]") + "%";
        var sql = @"
            SELECT * FROM Posts 
            WHERE Content LIKE @search
        ";
        return await connection.QueryAsync<Post>(sql, new { search });
    }

    public async Task Create(Post post)
    {
        using var connection = context.CreateConnection();
        var sql = @"
            IF NOT EXISTS (SELECT * FROM Posts 
                   WHERE Title = @Title)
            INSERT INTO Posts (Title, Content, PostedDate)
            VALUES (@Title, @Content, @PostedDate)
        ";
        await connection.ExecuteAsync(sql, post);
    }
}