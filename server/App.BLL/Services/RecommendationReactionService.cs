using App.Contracts.BLL.IServices;
using App.Contracts.DAL;
using App.Contracts.DAL.IRepositories;
using App.Domain.Identity;
using Base.BLL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
using DomainDto = App.Domain;
using AutoMappers = App.Mappers.AutoMappers;

namespace App.BLL.Services;

public class RecommendationReactionService :
    BaseEntityService<Dal.RecommendationReaction, Bll.RecommendationReaction, IRecommendationReactionRepository>, IRecommendationReactionService
{
    protected IAppUOW Uow;

    public RecommendationReactionService(IAppUOW uow, AutoMapper.IMapper mapper)
        : base(uow.RecommendationReactionRepository, new AutoMappers.BLL.RecommendationReactionMapper(mapper))
    {
        Uow = uow;
    }

    public async Task<Bll.RecommendationReaction?> Add(Guid reviewId, AppUser user, int userFeedback)
    {
        return Mapper.Map(
            await Uow.RecommendationReactionRepository.Add(reviewId, user, userFeedback)
        );
    }

    public Bll.RecommendationReaction? Update(Guid reviewId, AppUser user, int userFeedback)
    {
        return Mapper.Map(
            Uow.RecommendationReactionRepository.Update(reviewId, user, userFeedback)
        );
    }

    public bool Delete(Guid reviewId, AppUser user)
    {
        return Uow.RecommendationReactionRepository.Delete(reviewId, user);
    }
}