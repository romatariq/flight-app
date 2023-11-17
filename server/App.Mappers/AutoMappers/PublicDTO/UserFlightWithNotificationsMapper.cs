using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;
namespace App.Mappers.AutoMappers.PublicDTO;

public class UserFlightWithNotificationsMapper: BaseMapper<Bll.UserFlightWithNotifications, PublicV1.UserFlightWithNotifications>
{
    public UserFlightWithNotificationsMapper(IMapper mapper) : base(mapper)
    {
    }

}