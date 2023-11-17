namespace App.Public.DTO.v1.Identity;

/// <summary>
/// JWT response info.
/// </summary>
public class JWTResponse
{
    /// <summary>
    /// JWT token as string.
    /// </summary>
    public string JWT { get; set; } = default!;
    
    /// <summary>
    /// Refresh token as string.
    /// </summary>
    public string RefreshToken { get; set; } = default!;
}
