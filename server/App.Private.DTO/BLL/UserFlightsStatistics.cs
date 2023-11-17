namespace App.Private.DTO.BLL;

public class UserFlightsStatistics
{
    public int Count { get; set; }

    public int TotalDistance { get; set; }

    public int TotalTimeMinutes { get; set; }

    public int TotalTimeDelayedDepartureMinutes { get; set; }
    
    public int TotalTimeDelayedArrivalMinutes { get; set; }
}