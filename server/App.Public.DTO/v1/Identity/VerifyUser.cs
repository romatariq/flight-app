namespace App.Public.DTO.v1.Identity;

/// <summary>
/// Minimal info about user required to be verified by admin.
/// </summary>
public class VerifyUser
{
    /// <summary>
    /// Email of user.
    /// </summary>
    public string Email { get; set; } = default!;
    
    /// <summary>
    /// Verification status.
    /// </summary>
    public bool IsVerified { get; set; }
}