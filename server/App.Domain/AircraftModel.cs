using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace App.Domain;

public class AircraftModel: DomainEntityId
{

    [MaxLength(100)]
    public string ModelName { get; set; } = default!;
    
    [MaxLength(20)]
    public string ModelCode { get; set; } = default!;

    
    public ICollection<Aircraft>? Aircrafts { get; set; }
}