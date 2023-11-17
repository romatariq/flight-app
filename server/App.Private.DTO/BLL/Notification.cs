using Base.Domain;

namespace App.Private.DTO.BLL;

public class Notification: DomainEntityId
{
    public string Type { get; set; } = default!;
}