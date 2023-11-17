using Base.Domain;

namespace App.Private.DTO.BLL;

public class Airport: DomainEntityId
{
    public string Name { get; set; } = default!;

    public string Iata { get; set; } = default!;

    public bool DisplayFlights { get; set; }
    
    public string CountryName { get; set; } = default!;
    
    public string CountryIso2 { get; set; } = default!;
    
    public string CountryIso3 { get; set; } = default!;
}