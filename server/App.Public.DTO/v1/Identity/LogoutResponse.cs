namespace App.Public.DTO.v1.Identity;

/// <summary>
/// Log out response.
/// </summary>
public class LogoutResponse
{
    /// <summary>
    /// Amount of refresh tokens deleted.
    /// </summary>
    public int TokenDeleteCount { get; set; }
}