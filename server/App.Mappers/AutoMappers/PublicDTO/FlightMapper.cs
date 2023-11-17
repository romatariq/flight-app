using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;
namespace App.Mappers.AutoMappers.PublicDTO;


public class FlightMapper: BaseMapper<Bll.FlightInfo, PublicV1.Flight>
{
    public FlightMapper(IMapper mapper) : base(mapper)
    {
    }

}