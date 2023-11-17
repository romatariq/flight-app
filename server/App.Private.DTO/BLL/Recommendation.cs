using Base.Domain;

namespace App.Private.DTO.BLL;

public class Recommendation: DomainEntityId
{
    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = default!;

    public string AirportIata { get; set; } = default!;

    public string AuthorName { get; set; } = default!;
    
    public Guid AuthorId { get; set; }
    
    public string Text { get; set; } = default!;
    
    public DateTime CreatedAt { get; set; }
    
    public decimal Rating { get; set; }

    public bool? IsUserFeedbackPositive { get; set; }

    public int UsersFeedback { get; set; }
}