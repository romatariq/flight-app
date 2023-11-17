using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;
namespace App.Mappers.AutoMappers.PublicDTO;


public class RecommendationMapper: BaseMapper<Bll.Recommendation, PublicV1.Recommendation>
{
    public RecommendationMapper(IMapper mapper) : base(mapper)
    {
    }

    public override PublicV1.Recommendation? Map(Bll.Recommendation? entity)
    {
        if (entity == null) return null;
        
        var mapped = base.Map(entity);
        if (mapped == null) return null;
        
        mapped.UserFeedback = entity.IsUserFeedbackPositive == null ? 0 : (entity.IsUserFeedbackPositive.Value ? 1 : -1);
        mapped.Category = new PublicV1.RecommendationCategory
        {
            Id = entity.CategoryId,
            Category = entity.CategoryName
        };
        return mapped;
    }
}