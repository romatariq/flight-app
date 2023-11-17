using System.Net;

namespace App.Public.DTO.v1;

/// <summary>
/// Error response.
/// </summary>
public class RestApiErrorResponse
{
    /// <summary>
    /// Error http status code.
    /// </summary>
    public HttpStatusCode Status { get; set; }
    
    /// <summary>
    /// Error message.
    /// </summary>
    public string Error { get; set; } = default!;

}