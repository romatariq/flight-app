using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class FlightStatus: DomainEntityId
{
    
    [MaxLength(50)]
    public string Name { get; set; } = default!;


    public ICollection<Flight>? Flights { get; set; }
}