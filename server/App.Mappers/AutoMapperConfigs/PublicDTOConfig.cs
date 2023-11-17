using AutoMapper;
using PublicDTO = App.Public.DTO.v1;
using Bll = App.Private.DTO.BLL;

namespace App.Mappers.AutoMapperConfigs;

public class PublicDTOConfig : Profile
{
    public PublicDTOConfig()
    {
        CreateMap<Bll.Airport, PublicDTO.Airport>();
        
        CreateMap<Bll.AirportStatistics, PublicDTO.AirportStatistics>();
        
        CreateMap<Bll.FlightInfo, PublicDTO.Flight>();
        
        CreateMap<Bll.FlightInfoDetails, PublicDTO.FlightDetails>();

        CreateMap<Bll.Recommendation, PublicDTO.Recommendation>();
        
        CreateMap<Bll.UserFlightInfo, PublicDTO.UserFlight>();
        
        CreateMap<Bll.UserFlightNotificationInfo, PublicDTO.UserNotification>();
        
        CreateMap<Bll.UserFlightWithNotifications, PublicDTO.UserFlightWithNotifications>();
        
        
        CreateMap<Bll.Aircraft, PublicDTO.Aircraft>();
        
        CreateMap<Bll.NameCounter, PublicDTO.NameCounter>().ReverseMap();
        
        CreateMap<Bll.Notification, PublicDTO.Notification>().ReverseMap();
        
        CreateMap<Bll.UserFlightsStatistics, PublicDTO.UserFlightsStatistics>().ReverseMap();
    }
}