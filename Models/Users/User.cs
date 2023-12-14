using System.Text.Json.Serialization;

namespace NewsParser.Models.Users;

public class User
{
    public int Id { get; set; }
    public string? Login { get; set; }

    [JsonIgnore]
    public string? PasswordHash { get; set; }
}