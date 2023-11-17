using System.Net;
using System.Net.Mime;
using App.Contracts.BLL;
using App.Domain.Identity;
using Base.Helpers;
using App.Mappers.AutoMappers.PublicDTO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using AutoMapper;
using RecommendationMapper = App.Mappers.AutoMappers.PublicDTO.RecommendationMapper;
using Bll = App.Private.DTO.BLL;
using PublicV1 = App.Public.DTO.v1;

namespace WebApp.ApiControllers;

/// <summary>
/// Controller for managing reviews/recommendations.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ReviewsController: ControllerBase
{
    private readonly IAppBLL _uow;
    private readonly UserManager<AppUser> _userManager;
    private readonly RecommendationMapper _mapper;
    private readonly AirportMapper _airportMapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="uow">UWO from dependency injection.</param>
    /// <param name="userManager">User manager from dependency injection.</param>
    /// <param name="autoMapper">AutoMapper from dependency injection to create needed mapper.</param>
    public ReviewsController(IAppBLL uow, UserManager<AppUser> userManager, IMapper autoMapper)
    {
        _uow = uow;
        _userManager = userManager;
        _mapper = new RecommendationMapper(autoMapper);
        _airportMapper = new AirportMapper(autoMapper);
    }
    
    
    /// <summary>
    /// Gets reviews/recommendations for given airport and category.
    /// </summary>
    /// <param name="airportIata">Airport IATA (3 letter code) of airport to get reviews/recommendations from.</param>
    /// <param name="categoryId">Id of chosen category.</param>
    /// <param name="page">Optional, page number of reviews/recommendations.</param>
    /// <returns>Airport reviews/recommendations of requested category, page nr and total page nr.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PublicV1.Recommendations), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PublicV1.Recommendations>> GetAll(
        [FromQuery] string airportIata,
        [FromQuery] Guid categoryId,
        [FromQuery] int page = 1)
    {
        return await GetAllCommon(airportIata, categoryId, page);
    }    
        
    
    /// <summary>
    /// Gets reviews/recommendations for given airport and category.
    /// Also includes information about user's connection with the reviews/recommendations.
    /// </summary>
    /// <param name="airportIata">Airport IATA (3 letter code) of airport to get reviews/recommendations from.</param>
    /// <param name="categoryId">Id of chosen category.</param>
    /// <param name="page">Optional, page number of reviews/recommendations.</param>
    /// <returns>Airport reviews/recommendations of requested category, page nr and total page nr.</returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PublicV1.Recommendation), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PublicV1.Recommendations>> GetAllAuthorized(
        [FromQuery] string airportIata,
        [FromQuery] Guid categoryId,
        [FromQuery] int page = 1)
    {
        var userId = User.GetUserId();
        var appUser = await _userManager.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User not found"
            });
        }
        return await GetAllCommon(airportIata, categoryId, page, appUser);
    }
    

    /// <summary>
    /// Gets a review/recommendation by id.
    /// </summary>
    /// <param name="reviewId">Review id.</param>
    /// <returns>Information about requested review/recommendation.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PublicV1.Recommendation), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PublicV1.Recommendation>> Get(
        [FromQuery] Guid reviewId)
    {
        var review = await _uow.RecommendationService.GetRestDtoAsync(reviewId);

        if (review == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "Review not found"
            });
        }
        
        return _mapper.Map(review)!;
    }


    /// <summary>
    /// Adds a new review/recommendation.
    /// </summary>
    /// <param name="recommendation">Information required to add a review/recommendation.</param>
    /// <returns>Created review/recommendation.</returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Add(
        [FromBody] PublicV1.Recommendation recommendation)
    {
        if (recommendation.Rating < 1 || recommendation.Rating > 5)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Rating must be between 1 and 5"
            });
        }
        
        if (recommendation.Text.Length < 1 || recommendation.Text.Length > 1000)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Rating must be between 1 and 1000 chars"
            });
        }

        var userId = User.GetUserId();
        var appUser = await _userManager.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User not found"
            });
        }
        
        
        var res = await _uow.RecommendationService
            .AddAsync(recommendation.Category.Id, recommendation.AirportIata, recommendation.Rating, recommendation.Text, appUser);

        if (res == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "Something went wrong"
            });
        }
        
        await _uow.SaveChangesAsync();
        return res;
    }

    
    /// <summary>
    /// Updates a review/recommendation.
    /// </summary>
    /// <param name="recommendation">Information required to update a review/recommendation.</param>
    /// <returns>Updated review/recommendation.</returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update(
        [FromBody] PublicV1.Recommendation recommendation)
    {
        if (recommendation.Rating < 1 || recommendation.Rating > 5)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Rating must be between 1 and 5"
            });
        }
        if (recommendation.Text.Length < 1 || recommendation.Text.Length > 1000)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Rating must be between 1 and 1000 chars"
            });
        }
        if (recommendation.Id == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Missing review id"
            });
        }

        var userId = User.GetUserId();
        var appUser = await _userManager.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User not found"
            });
        }

        var res= await _uow.RecommendationService
            .UpdateAsync(recommendation.Id.Value, recommendation.Category.Id, recommendation.Rating, recommendation.Text, appUser);
        if (!res)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "Something went wrong"
            });
        }
        await _uow.SaveChangesAsync();
        return Ok();
    }
    
    
    /// <summary>
    /// Deletes a review/recommendation.
    /// </summary>
    /// <param name="id">Id of the review/recommendation to delete.</param>
    /// <returns>Ok if delete successful.</returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
    [FromQuery] Guid id)
    {
        var userId = User.GetUserId();
        var appUser = await _userManager.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User not found"
            });
        }

        var res = await _uow.RecommendationService.DeleteAsync(id);
        if (!res)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "something went wrong"
            });
        }
        await _uow.SaveChangesAsync();
        return Ok();
    }


    /// <summary>
    /// Gets all categories.
    /// </summary>
    /// <returns>Categories.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<PublicV1.RecommendationCategory>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PublicV1.RecommendationCategory>>> Categories()
    {
        var categories = await _uow.RecommendationCategoryService.GetAllAsync();
        var formattedCategories = categories
            .Select(c => new PublicV1.RecommendationCategory()
            {
                Id = c.Id,
                Category = c.Category
            })
            .ToList();
        return formattedCategories;
    }
    
    
    private async Task<ActionResult<PublicV1.Recommendations>> GetAllCommon(string airportIata, Guid categoryId, int page = 1, AppUser? appUser = null)
    {
        const int itemsPerPage = 10;

        IEnumerable<Bll.Recommendation> reviews;
        int totalPageCount;

        try
        {
            totalPageCount = _uow.RecommendationService.GetAllPageCount(airportIata, categoryId, itemsPerPage);
            if (totalPageCount < page || page < 1)
            {
                return BadRequest(new PublicV1.RestApiErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = $"Invalid page number {page}/{totalPageCount}"
                });
            }
            reviews = await _uow.RecommendationService.GetAllAsync(airportIata, categoryId, page, itemsPerPage, appUser);
        }
        catch (Exception e)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse
            {
                Status = HttpStatusCode.BadRequest,
                Error = $"{e.Message}"
            });
        }
        

        var formattedReviews = reviews
            .Select(_mapper.Map)
            .ToList();


        var airport = await _uow.AirportService.GetByIata(airportIata);

        return new PublicV1.Recommendations
        {
            Data = formattedReviews!,
            Page = page,
            TotalPageCount = totalPageCount,
            Airport = _airportMapper.Map(airport)!
        };
    }

}