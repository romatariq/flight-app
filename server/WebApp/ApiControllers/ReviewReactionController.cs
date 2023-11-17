using System.Net;
using System.Net.Mime;
using App.Contracts.BLL;
using App.Domain.Identity;
using Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asp.Versioning;
using PublicV1 = App.Public.DTO.v1;

namespace WebApp.ApiControllers;

/// <summary>
/// Controller for managing reactions of reviews/recommendations.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class ReviewReactionController: ControllerBase
{
    private readonly IAppBLL _uow;
    private readonly UserManager<AppUser> _userManager;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="uow">UOW from dependency injection.</param>
    /// <param name="userManager">User manager from dependency injection.</param>
    public ReviewReactionController(IAppBLL uow, UserManager<AppUser> userManager)
    {
        _uow = uow;
        _userManager = userManager;
    }
    
    
    /// <summary>
    /// Adds a reaction to a review/recommendation.
    /// </summary>
    /// <param name="request">Info required to add a reaction.</param>
    /// <returns>Ok if adding successful.</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Add([FromBody] PublicV1.ReviewReaction request)
    {
        if (request.Feedback is not( -1 or 1))
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "User feedback must be either -1 or -1"
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
        

        var reaction = await _uow.RecommendationReactionService.Add(request.ReviewId, appUser, request.Feedback);
        if (reaction == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Something went wrong"
            });
        }
        await _uow.SaveChangesAsync();
        return Ok();
    }
    
    
    /// <summary>
    /// Updates a reaction to a review/recommendation.
    /// </summary>
    /// <param name="request">Info required to update a reaction..</param>
    /// <returns>Ok if update successful.</returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Update([FromBody] PublicV1.ReviewReaction request)
    {
        if (request.Feedback is not( -1 or 1))
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "User feedback must be either -1 or -1"
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

        var reaction = _uow.RecommendationReactionService.Update(request.ReviewId, appUser, request.Feedback);
        if (reaction == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Something went wrong"
            });
        }
        await _uow.SaveChangesAsync();
        return Ok();
    }
    
    
    /// <summary>
    /// Deletes a reaction to a review/recommendation.
    /// </summary>
    /// <param name="reviewId">Id of the review from where to delete user reaction.</param>
    /// <returns>Ok if delete successful.</returns>
    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete([FromQuery] Guid reviewId)
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

        var reaction = _uow.RecommendationReactionService.Delete(reviewId, appUser);
        if (!reaction)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Something went wrong"
            });
        }
        await _uow.SaveChangesAsync();
        return Ok();
    }
}