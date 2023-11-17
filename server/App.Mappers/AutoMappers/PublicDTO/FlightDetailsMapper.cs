using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;
namespace App.Mappers.AutoMappers.PublicDTO;


public class FlightDetailsMapper: BaseMapper<Bll.FlightInfoDetails, PublicV1.FlightDetails>
{
    public FlightDetailsMapper(IMapper mapper) : base(mapper)
    {
    }

}