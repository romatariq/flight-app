using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class Country: DomainEntityId
{

    [MaxLength(100)]
    public string Name { get; set; } = default!;

    [StringLength(2)] 
    public string Iso2 { get; set; } = default!;
    
    [StringLength(3)] 
    public string Iso3 { get; set; } = default!;

    
    public ICollection<Airport>? Airports { get; set; }
}