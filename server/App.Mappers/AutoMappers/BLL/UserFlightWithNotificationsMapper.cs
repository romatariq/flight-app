using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
namespace App.Mappers.AutoMappers.BLL;

public class UserFlightWithNotificationsMapper: BaseMapper<Dal.UserFlightWithNotifications, Bll.UserFlightWithNotifications>
{
    public UserFlightWithNotificationsMapper(IMapper mapper) : base(mapper)
    {
    }

}