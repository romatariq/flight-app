using Base.Domain;

namespace App.Private.DTO.DAL;

public class Airport: DomainEntityId
{
    public string Name { get; set; } = default!;

    public string Iata { get; set; } = default!;

    public bool DisplayFlights { get; set; }
    
    public string CountryName { get; set; } = default!;
    
    public string CountryIso2 { get; set; } = default!;
    
    public string CountryIso3 { get; set; } = default!;

    public DateTime? DeparturesLastCheckedUtc { get; set; }
    
    public DateTime? ArrivalsLastCheckedUtc { get; set; }
}