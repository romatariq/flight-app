using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
namespace App.Mappers.AutoMappers.BLL;


public class RecommendationCategoryMapper: BaseMapper<Dal.RecommendationCategory, Bll.RecommendationCategory>
{
    public RecommendationCategoryMapper(IMapper mapper) : base(mapper)
    {
    }

}