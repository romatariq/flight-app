using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;
namespace App.Mappers.AutoMappers.PublicDTO;


public class UserFlightMapper: BaseMapper<Bll.UserFlightInfo, PublicV1.UserFlight>
{
    public UserFlightMapper(IMapper mapper) : base(mapper)
    {
    }

}