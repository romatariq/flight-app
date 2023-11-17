using AutoMapper;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;

namespace App.Mappers.AutoMapperConfigs;

public class BLLConfig: Profile
{
    public BLLConfig()
    {
        CreateMap<Dal.Airport, Bll.Airport>();
        
        CreateMap<Dal.AirportStatistics, Bll.AirportStatistics>();
        
        CreateMap<Dal.FlightInfo, Bll.FlightInfo>();
        
        CreateMap<Dal.FlightInfoDetails, Bll.FlightInfoDetails>();

        CreateMap<Dal.Recommendation, Bll.Recommendation>();
        
        CreateMap<Dal.RecommendationCategory, Bll.RecommendationCategory>();
        
        CreateMap<Dal.RecommendationReaction, Bll.RecommendationReaction>();
        
        CreateMap<Dal.UserFlightInfo, Bll.UserFlightInfo>();
        
        CreateMap<Dal.UserFlightNotificationInfo, Bll.UserFlightNotificationInfo>();
        
        CreateMap<Dal.UserFlightWithNotifications, Bll.UserFlightWithNotifications>();
        
        
        CreateMap<Dal.Aircraft, Bll.Aircraft>().ReverseMap();
        
        CreateMap<Dal.NameCounter, Bll.NameCounter>().ReverseMap();
        
        CreateMap<Dal.Notification, Bll.Notification>().ReverseMap();
    }
}
