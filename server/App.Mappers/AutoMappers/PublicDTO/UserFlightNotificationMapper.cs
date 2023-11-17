using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;
namespace App.Mappers.AutoMappers.PublicDTO;


public class UserFlightNotificationMapper: BaseMapper<Bll.UserFlightNotificationInfo, PublicV1.UserNotification>
{
    public UserFlightNotificationMapper(IMapper mapper) : base(mapper)
    {
    }

}