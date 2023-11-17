using Base.Domain;

namespace App.Private.DTO.DAL;

public class Notification: DomainEntityId
{
    public string Type { get; set; } = default!;
}