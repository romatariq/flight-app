using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;

namespace App.Mappers.AutoMappers.PublicDTO;


public class UserFlightsStatisticsMapper: BaseMapper<Bll.UserFlightsStatistics, PublicV1.UserFlightsStatistics>
{
    public UserFlightsStatisticsMapper(IMapper mapper) : base(mapper)
    {
    }

}