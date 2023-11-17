using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
namespace App.Mappers.AutoMappers.BLL;


public class UserFlightNotificationMapper: BaseMapper<Dal.UserFlightNotificationInfo, Bll.UserFlightNotificationInfo>
{
    public UserFlightNotificationMapper(IMapper mapper) : base(mapper)
    {
    }

}