using App.Contracts.DAL.IRepositories;
using Base.DAL.EF;
using App.Domain;
using App.Domain.Identity;
using Base.Helpers;
using Microsoft.EntityFrameworkCore;
using Dal = App.Private.DTO.DAL;

namespace App.DAL.EF.Repositories;

public class RecommendationRepository: EFBaseRepository<Recommendation, AppDbContext>, IRecommendationRepository
{
    public RecommendationRepository(AppDbContext dbContext) : base(dbContext)
    {
    }


    public async Task<Dal.Recommendation?> GetRestDtoAsync(Guid id)
    {
        return await DbSet
            .Include(r => r.Airport)
            .Include(r => r.AppUser)
            .Include(r => r.RecommendationCategory)
            .Where(r => r.Id == id)
            .Select(r => new Private.DTO.DAL.Recommendation
            {
                Id = r.Id,
                Text = r.RecommendationText,
                Rating = r.Rating,
                CategoryId = r.RecommendationCategoryId,
                CategoryName = r.RecommendationCategory!.Category,
                AirportIata = r.Airport!.Iata,
                AuthorName = r.AppUser!.FirstName + " " + r.AppUser.LastName,
                AuthorId = r.AppUserId,
                CreatedAt = r.CreatedAtUtc
            })
            .SingleOrDefaultAsync();
    }

    public async Task<Guid?> AddAsync(Guid categoryId, string airportIata, decimal rating, string text, AppUser author)
    {
        var airport = await DbContext.Airports
            .FirstOrDefaultAsync(a => a.Iata == airportIata.ToUpper());
        if (airport == null) return null;
        
        var category = await DbContext.RecommendationCategories.FindAsync(categoryId);
        if (category == null) return null;

        var review = new Domain.Recommendation
        {
            RecommendationText = text,
            Rating = rating,
            RecommendationCategoryId = categoryId,
            AirportId = airport.Id,
            AppUserId = author.Id
        };
        var result = (await DbSet.AddAsync(review)).Entity;
        return result.Id;
    }

    public async Task<bool> UpdateAsync(Guid id, Guid categoryId, decimal rating, string text, AppUser author)
    {
        var category = await DbContext.RecommendationCategories.FindAsync(categoryId);
        if (category == null) return false;
        
        var review = await DbSet.FindAsync(id);
        if (review == null || review.AppUserId != author.Id) return false;
        
        review.RecommendationText = text;
        review.Rating = rating;
        review.RecommendationCategoryId = categoryId;
        DbSet.Update(review);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, AppUser author)
    {
        var review = await DbSet.FindAsync(id);
        if (review == null || review.AppUserId != author.Id) return false;

        DbSet.Remove(review);
        return true;
    }

    public async Task<IEnumerable<Dal.Recommendation>> GetAllAsync(string airportIata, Guid categoryId, int page = 1, int pageSize = 10, AppUser? user = null)
    {
        var airport = await DbContext.Airports
            .FirstOrDefaultAsync(a => a.Iata == airportIata.ToUpper());
        if (airport == null)
        {
            throw new Exception("Airport not found");
        }
        var category = await DbContext.RecommendationCategories.FindAsync(categoryId);
        if (category == null)
        {
            throw new Exception("Category not found");
        }
        
        var userId = user?.Id ?? null;
        return await DbSet
            .Include(r => r.AppUser)
            .Include(r => r.RecommendationCategory)
            .Include(r => r.Airport)
            .Include(r => r.RecommendationReactions!)
            .OrderByDescending(r => r.CreatedAtUtc)
            .Where(r => 
                r.AirportId == airport.Id &&
                r.RecommendationCategoryId == category.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new Dal.Recommendation
            {
                Id = r.Id,
                Text = r.RecommendationText,
                Rating = r.Rating,
                CategoryId = r.RecommendationCategoryId,
                CategoryName = r.RecommendationCategory!.Category,
                AirportIata = r.Airport!.Iata,
                AuthorName = r.AppUser!.FirstName + " " + r.AppUser.LastName,
                AuthorId = r.AppUserId,
                CreatedAt = r.CreatedAtUtc,
                UsersFeedback = 
                    r.RecommendationReactions!.Count(rr => rr.IsPositiveReaction) - 
                    r.RecommendationReactions!.Count(rr => !rr.IsPositiveReaction),
                IsUserFeedbackPositive = 
                    r.RecommendationReactions!.FirstOrDefault(rt => rt.AppUserId == userId) == null ? 
                        null :
                        r.RecommendationReactions!.FirstOrDefault(rt => rt.AppUserId == userId)!.IsPositiveReaction
            })
            .ToListAsync();
    }

    public int GetAllPageCount(string airportIata, Guid categoryId, int pageSize = 10)
    {
        var airport = DbContext.Airports
            .FirstOrDefault(a => a.Iata == airportIata.ToUpper());
        if (airport == null)
        {
            throw new Exception("Airport not found");
        }
        
        var recommendationsCount = DbSet
            .OrderByDescending(r => r.CreatedAtUtc)
            .Where(r => r.RecommendationCategoryId == categoryId)
            .Count(r => r.AirportId == airport.Id);
        return EFHelpers.GetPageCount(recommendationsCount, pageSize);
    }
}