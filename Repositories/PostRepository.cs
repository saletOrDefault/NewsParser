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
        Regex regex = new Regex(@"[^a-zа-яё\s]");
        post.CleanedContent = regex.Replace(post.Content!.ToLower(CultureInfo.InvariantCulture), "").Replace("\n", "").Trim();
        var sql = @"
            INSERT INTO Posts (Title, Content, PostedDate, CleanedContent)
            VALUES (@Title, @Content, @PostedDate, @CleanedContent)
        ";
        try
        {
            await connection.ExecuteAsync(sql, post);
        }
        catch (SqlException ex) when (ex.Message.Contains("Violation of UNIQUE KEY constraint"))
        {
            // skip duplicate
        }
    }

    public async Task<IEnumerable<PostTopResponse>> TopTen()
    {
        using var connection = context.CreateConnection();
        var sql = @"
            SELECT TOP 10 value as word, COUNT(*) AS [NumberOfOccurence]
            FROM Posts
            CROSS APPLY STRING_SPLIT(CleanedContent, ' ')
            GROUP BY value
            HAVING LEN(value) > 0
            ORDER BY COUNT(*) DESC
            ";
        return await connection.QueryAsync<PostTopResponse>(sql);
    }
}