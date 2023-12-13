using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using NewsParser.Helpers;
using NewsParser.Models.Posts;

namespace NewsParser.Parsers;

public static class PageParser
{
    private static readonly List<string> knownTags = new List<string> { "<p>", "</p>", "<strong>", "</strong>", "</br>", "<em>", "</em>" };

    public static Post Parse(string fileContent)
    {
        var html = HttpUtility.HtmlDecode(fileContent).Split('\n');
        var title = ParseTitle(html);
        var date = ParseDate(html);
        var content = ParseContent(html);
        return new Post
        {
            Title = title,
            PostedDate = date,
            Content = content
        };
    }

    private static string ParseTitle(string[] html)
    {
        string title = html.First(x => x.Contains("<title>"));
        string toBeSearched = "<title>";
        int index = title.IndexOf(toBeSearched);
        if (index != -1)
        {
            string line = title[(index + toBeSearched.Length)..];
            return line.Split(" |").First();
        }
        else throw new MyException("Title not found");
    }

    private static DateTime ParseDate(string[] html)
    {
        string date = html.First(x => x.Contains("<meta property=\"article:published_time\""));
        string toBeSearched = "content=\"";
        int index = date.IndexOf(toBeSearched);
        if (index != -1)
        {
            string line = date[(index + toBeSearched.Length)..];
            return DateTime.Parse(line.Replace("\">", ""));
        }
        else throw new MyException("Date not found");
    }

    private static string ParseContent(string[] html)
    {
        var index = Array.FindIndex(html, row => row == "<div class=\"article\">");
        var post = new StringBuilder();
        for (int i = index + 1; i < html.Length; i++)
        {
            if (html[i].Contains("Читайте также:"))
            {
                break;
            }
            if (html[i].Contains("<p>") && !html[i].Contains("<img") && !html[i].Contains("<figure"))
            {
                var sb = new StringBuilder(html[i]);
                string line = "";
                foreach (var tag in knownTags)
                {
                    sb.Replace(tag, "");
                }
                if (html[i].Contains("</a>"))
                {
                    sb.Replace("</a>", "");
                    Regex regex = new Regex("<a.*\">");
                    line = regex.Replace(sb.ToString(), "");
                }
                else
                {
                    line = sb.ToString();
                }
                post.AppendLine(line).AppendLine();
            }
        }
        return post.ToString();
    }
}