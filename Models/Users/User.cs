namespace NewsParser.Models.Users;

using System.Text.Json.Serialization;

public class User
{
    public int Id { get; set; }
    public string? Login { get; set; }

    [JsonIgnore]
    public string? PasswordHash { get; set; }
}