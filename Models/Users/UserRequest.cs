namespace NewsParser.Models.Users;

using System.ComponentModel.DataAnnotations;

public class UserRequest
{
    [Required]
    public string? Login { get; set; }

    [Required]
    public string? Password { get; set; }
}