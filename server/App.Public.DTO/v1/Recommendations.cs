using Base.Public.DTO;

namespace App.Public.DTO.v1;

/// <summary>
/// List of recommendations with additional data, such as airport info, page number, total page number.
/// </summary>
public class Recommendations: RestApiResponseWithPaging<Recommendation>
{
    /// <summary>
    /// Information about the airport recommendations/reviews are about.
    /// </summary>
    public Airport Airport { get; set; } = default!;
}