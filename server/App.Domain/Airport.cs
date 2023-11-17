using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Base.Domain;

namespace App.Domain;

public class Airport: DomainEntityId
{
    
    [StringLength(3)]
    public string Iata { get; set; } = default!;
    
    [MaxLength(200)]
    public string Name { get; set; } = default!;
    
    public DateTime? DeparturesLastCheckedUtc { get; set; }
    
    public DateTime? ArrivalsLastCheckedUtc { get; set; }

    public double Longitude { get; set; }
    
    public double Latitude { get; set; }

    public bool DisplayGate { get; set; }
    
    public bool DisplayTerminal { get; set; }

    public bool DisplayAirport { get; set; } = false;
    
    
    public Guid CountryId { get; set; }
    public Country? Country { get; set; }
    
    [InverseProperty(nameof(Flight.DepartureAirport))]
    public ICollection<Flight>? DepartureFlights { get; set; }

    [InverseProperty(nameof(Flight.ArrivalAirport))]
    public ICollection<Flight>? ArrivalFlights { get; set; }

    public ICollection<Recommendation>? Recommendations { get; set; }
}