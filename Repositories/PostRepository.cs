using System.Data;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using Microsoft.Data.SqlClient;
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
        var procedure = "SearchPosts";
        var search = "%" + text.Replace("[", "[[]").Replace("%", "[%]") + "%";
        using var connection = context.CreateConnection();
        return await connection.QueryAsync<Post>(procedure, new { search }, commandType: CommandType.StoredProcedure);
    }

    public async Task Create(Post post)
    {
        var procedure = "AddPost";
        using var connection = context.CreateConnection();
        Regex regex = new Regex(@"[^a-zа-яё\s]");
        post.CleanedContent = regex.Replace(post.Content!.ToLower(CultureInfo.InvariantCulture), "").Replace("\n", "").Trim();
        try
        {
            await connection.ExecuteAsync(procedure, new { post.Title, post.Content, post.PostedDate, post.CleanedContent }, commandType: CommandType.StoredProcedure);
        }
        catch (SqlException ex) when (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
        {
            // skip duplicate
        }
    }

    public async Task<IEnumerable<PostTopResponse>> TopTen()
    {
        var procedure = "TopTen";
        using var connection = context.CreateConnection();
        return await connection.QueryAsync<PostTopResponse>(procedure, commandType: CommandType.StoredProcedure);
    }
}