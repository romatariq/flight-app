using App.Contracts.BLL.IServices;
using App.Contracts.DAL;
using App.Contracts.DAL.IRepositories;
using Base.BLL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
using DomainDto = App.Domain;
using AutoMappers = App.Mappers.AutoMappers;

namespace App.BLL.Services;

public class RecommendationCategoryService :
    BaseEntityService<Dal.RecommendationCategory, Bll.RecommendationCategory, IRecommendationCategoryRepository>,
    IRecommendationCategoryService
{
    protected IAppUOW Uow;

    public RecommendationCategoryService(IAppUOW uow, AutoMapper.IMapper mapper)
        : base(uow.RecommendationCategoryRepository, new AutoMappers.BLL.RecommendationCategoryMapper(mapper))
    {
        Uow = uow;
    }


    public async Task<IEnumerable<Bll.RecommendationCategory>> GetAllAsync()
    {
        return (await Uow.RecommendationCategoryRepository.GetAllAsync())
            .Select(Mapper.Map)!;
    }
}