using App.Contracts.DAL.IRepositories;
using Base.DAL.EF;
using App.Domain;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class RecommendationCategoryRepository: EFBaseRepository<RecommendationCategory, AppDbContext>, IRecommendationCategoryRepository
{
    public RecommendationCategoryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Private.DTO.DAL.RecommendationCategory>> GetAllAsync()
    {
        return (await DbSet.ToListAsync())
            .Select(c => new Private.DTO.DAL.RecommendationCategory
            {
                Category = c.Category,
                Id = c.Id
            });
    }
}