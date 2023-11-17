using System.ComponentModel.DataAnnotations;

namespace App.Public.DTO.v1.Identity;

/// <summary>
/// Info required to log in.
/// </summary>
public class Login
{
    /// <summary>
    /// Email of user.
    /// </summary>
    [StringLength(maximumLength:128, MinimumLength = 5, ErrorMessage = "Wrong length on email")] 
    public string Email { get; set; } = default!;
    
    /// <summary>
    /// Password of user.
    /// </summary>
    public string Password { get; set; } = default!;
}
