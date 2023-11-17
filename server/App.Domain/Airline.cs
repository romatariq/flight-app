using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Airline: DomainEntityId
{
    
    [MaxLength(100)]
    public string Name { get; set; } = default!;
    
    [MaxLength(5)]
    public string Iata { get; set; } = default!;
    
    [MaxLength(5)]
    public string Icao { get; set; } = default!;

    public byte[]? Logo { get; set; }


    public ICollection<Flight>? Flights { get; set; }
}