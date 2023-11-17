using AutoMapper;
using Base.DAL;
using Base.Helpers;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
namespace App.Mappers.AutoMappers.BLL;


public class FlightMapper: BaseMapper<Dal.FlightInfo, Bll.FlightInfo>
{
    public FlightMapper(IMapper mapper) : base(mapper)
    {
    }


    public override Bll.FlightInfo? Map(Dal.FlightInfo? entity)
    {
        if (entity == null) return null;
        
        var mapped = base.Map(entity)!;
        
        mapped.ScheduledDepartureLocal = entity.ScheduledDepartureUtc
            .ConvertDateTimeFromUtc((entity.DepartureAirportLatitude, entity.DepartureAirportLongitude));
        mapped.ScheduledArrivalLocal = entity.ScheduledArrivalUtc
            .ConvertDateTimeFromUtc((entity.ArrivalAirportLatitude, entity.ArrivalAirportLongitude));
        return mapped;

    }
}