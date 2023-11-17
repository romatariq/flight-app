namespace App.Public.DTO;

/// <summary>
/// Object for sending id in request body.
/// </summary>
public class RestApiId
{
    /// <summary>
    /// Identification.
    /// </summary>
    public Guid Id { get; set; }
}