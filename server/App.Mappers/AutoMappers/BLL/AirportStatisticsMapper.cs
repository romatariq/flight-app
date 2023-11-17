using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;

namespace App.Mappers.AutoMappers.BLL;

public class AirportStatisticsMapper: BaseMapper<Dal.AirportStatistics, Bll.AirportStatistics>
{
    public AirportStatisticsMapper(IMapper mapper) : base(mapper)
    {
    }

}