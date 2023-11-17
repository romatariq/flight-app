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

public class RecommendationService :
    BaseEntityService<Dal.Recommendation, Bll.Recommendation, IRecommendationRepository>, IRecommendationService
{
    protected IAppUOW Uow;

    public RecommendationService(IAppUOW uow, AutoMapper.IMapper mapper)
        : base(uow.RecommendationRepository, new AutoMappers.BLL.RecommendationMapper(mapper))
    {
        Uow = uow;
    }

    
    public async Task<IEnumerable<Bll.Recommendation>> GetAllAsync(string airportIata, Guid categoryId, int page = 1, int pageSize = 10, AppUser? user = null)
    {
        return (await Uow.RecommendationRepository
            .GetAllAsync(airportIata, categoryId, page, pageSize, user))
            .Select(Mapper.Map)!;
    }

    public int GetAllPageCount(string airportIata, Guid categoryId, int pageSize = 10)
    {
        return Uow.RecommendationRepository.GetAllPageCount(airportIata, categoryId, pageSize);
    }

    public async Task<Bll.Recommendation?> GetRestDtoAsync(Guid id)
    {
        return Mapper.Map(
            await Uow.RecommendationRepository.GetRestDtoAsync(id)
        );
    }

    public async Task<Guid?> AddAsync(Guid categoryId, string airportIata, decimal rating, string text, AppUser author)
    {
        return await Uow.RecommendationRepository.AddAsync(categoryId, airportIata, rating, text, author);
    }

    public async Task<bool> UpdateAsync(Guid id, Guid categoryId, decimal rating, string text, AppUser author)
    {
        return await Uow.RecommendationRepository.UpdateAsync(id, categoryId, rating, text, author);
    }

    public async Task<bool> DeleteAsync(Guid id, AppUser author)
    {
        return await Uow.RecommendationRepository.DeleteAsync(id, author);
    }
}