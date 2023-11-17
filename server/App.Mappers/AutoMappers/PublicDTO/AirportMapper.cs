using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;

namespace App.Mappers.AutoMappers.PublicDTO;

public class AirportMapper: BaseMapper<Bll.Airport, PublicV1.Airport>
{
    public AirportMapper(IMapper mapper) : base(mapper)
    {
    }

}