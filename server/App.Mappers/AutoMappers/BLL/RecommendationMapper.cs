using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
namespace App.Mappers.AutoMappers.BLL;


public class RecommendationMapper: BaseMapper<Dal.Recommendation, Bll.Recommendation>
{
    public RecommendationMapper(IMapper mapper) : base(mapper)
    {
    }

}