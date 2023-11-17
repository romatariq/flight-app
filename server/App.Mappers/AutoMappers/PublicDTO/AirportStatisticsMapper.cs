using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;

namespace App.Mappers.AutoMappers.PublicDTO;

public class AirportStatisticsMapper: BaseMapper<Bll.AirportStatistics, PublicV1.AirportStatistics>
{
    public AirportStatisticsMapper(IMapper mapper) : base(mapper)
    {
    }

}