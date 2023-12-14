using System.ComponentModel.DataAnnotations;

namespace NewsParser.Models.Users;

public class UserRequest
{
    [Required]
    public string? Login { get; set; }

    [Required]
    public string? Password { get; set; }
}