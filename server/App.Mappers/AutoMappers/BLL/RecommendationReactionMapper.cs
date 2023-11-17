using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
namespace App.Mappers.AutoMappers.BLL;


public class RecommendationReactionMapper: BaseMapper<Dal.RecommendationReaction, Bll.RecommendationReaction>
{
    public RecommendationReactionMapper(IMapper mapper) : base(mapper)
    {
    }

}