namespace App.Public.DTO.v1;

/// <summary>
/// Class to replace (string, int) tuple.
/// </summary>
public class NameCounter
{
    /// <summary>
    /// Name of something.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Count of items with given name.
    /// </summary>
    public int Count { get; set; }
}