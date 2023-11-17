using System.ComponentModel.DataAnnotations;

namespace App.Public.DTO.v1.Identity;

/// <summary>
/// Info required to register a new user
/// </summary>
public class Register
{
    /// <summary>
    /// Email of user.
    /// </summary>
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Incorrect length")]
    public string Email { get; set; } = default!;
    
    /// <summary>
    /// Password of user.
    /// </summary>
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Incorrect length")]
    public string Password { get; set; } = default!;    
    
    /// <summary>
    /// First name of user.
    /// </summary>
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Incorrect length")]
    public string FirstName { get; set; } = default!;
    
    /// <summary>
    /// Last name of user.
    /// </summary>
    [StringLength(128, MinimumLength = 1, ErrorMessage = "Incorrect length")]
    public string LastName { get; set; } = default!;
}
