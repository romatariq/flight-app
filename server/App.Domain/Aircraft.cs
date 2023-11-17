using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Aircraft: DomainEntityId
{
    [MaxLength(10)]
    public string IcaoHex { get; set; } = default!;

    [MaxLength(10)]
    public string RegistrationNumber { get; set; } = default!;

    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }

    public decimal SpeedKmh { get; set; }

    public DateTime InfoLastUpdatedUtc { get; set; }
    

    public Guid AircraftModelId { get; set; }
    public AircraftModel? AircraftModel { get; set; }

    public ICollection<Flight>? Flights { get; set; }
}