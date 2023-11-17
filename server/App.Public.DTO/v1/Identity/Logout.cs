namespace App.Public.DTO.v1.Identity;

/// <summary>
/// Info required to log out.
/// </summary>
public class Logout
{
    /// <summary>
    /// Refresh token as string.
    /// </summary>
    public string RefreshToken { get; set; } = default!;
}