namespace App.Public.DTO.v1;

/// <summary>
/// Airport info.
/// </summary>
public class Airport
{
    /// <summary>
    /// Identification.
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Name of airport.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Iata (3 letter code) of airport.
    /// </summary>
    public string Iata { get; set; } = default!;

    /// <summary>
    /// Should airport display flights (since resources are minimal).
    /// </summary>
    public bool DisplayFlights { get; set; }
    
    /// <summary>
    /// Name of the country airport is located in.
    /// </summary>
    public string CountryName { get; set; } = default!;
    
    /// <summary>
    /// Iso2 of the country airport is located in.
    /// </summary>
    public string CountryIso2 { get; set; } = default!;
    
    /// <summary>
    /// Iso3 of the country airport is located in.
    /// </summary>
    public string CountryIso3 { get; set; } = default!;
}