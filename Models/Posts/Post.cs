using System.Text.Json.Serialization;

namespace NewsParser.Models.Posts
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime PostedDate { get; set; }

        [JsonIgnore]
        public string? CleanedContent { get; set; }
    }
}