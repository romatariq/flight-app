using System.Net;
using System.Net.Mime;
using App.Contracts.BLL;
using App.Domain.Identity;
using App.Mappers.AutoMappers.PublicDTO;
using App.Public.DTO;
using Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Public.DTO.v1;
using Asp.Versioning;
using AutoMapper;

namespace WebApp.ApiControllers;

/// <summary>
/// Controller for managing user's tracked flights.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class UserFlightsController: ControllerBase
{
    private readonly IAppBLL _uow;
    private readonly UserManager<AppUser> _userManager;
    private readonly UserFlightMapper _mapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="uow">UOW from dependency injection.</param>
    /// <param name="userManager">User manager from dependency injection.</param>
    /// <param name="autoMapper">AutoMapper from dependency injection to create needed mapper.</param>
    public UserFlightsController(IAppBLL uow, UserManager<AppUser> userManager, IMapper autoMapper)
    {
        _uow = uow;
        _userManager = userManager;
        _mapper = new UserFlightMapper(autoMapper);
    }
    
    
    /// <summary>
    /// Adds a flight to user's tracked flights.
    /// </summary>
    /// <param name="flightRequest">Id of the flight to be tracked.</param>
    /// <returns>The id of userFlight.</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Add([FromBody] RestApiId flightRequest)
    {
        var userId = User.GetUserId();
        var appUser = await _userManager.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User not found"
            });
        }
        
        
        Guid userFlightId;

        try
        {
            userFlightId = await _uow.UserFlightService.AddAsync(appUser, flightRequest.Id);
        } 
        catch(Exception e)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = $"{e.Message}"
            });
        }
        
        await _uow.SaveChangesAsync();
        return userFlightId;
    }    
    
    
    /// <summary>
    /// Removes a flight from user's tracked flights.
    /// </summary>
    /// <param name="userFlightId">Id of userFlight.</param>
    /// <returns>Ok if delete successful.</returns>
    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete([FromQuery] Guid userFlightId)
    {
        var userId = User.GetUserId();
        var appUser = await _userManager.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User not found"
            });
        }
        
        var res = await _uow.UserFlightService.DeleteAsync(userFlightId, appUser);
        if (!res)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "Failed to delete"
            });
        }
        await _uow.SaveChangesAsync();
        return Ok();
    }    
    
    
    /// <summary>
    /// Gets all user's tracked flights.
    /// </summary>
    /// <returns>User tracked flights as <see cref="UserFlight"/>.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<UserFlight>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<UserFlight>>> GetAll()
    {
        var userId = User.GetUserId();
        var appUser = await _userManager.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User not found"
            });
        }
        
        var userFlights = await _uow.UserFlightService.GetAllAsync(appUser);
        return userFlights
            .Select(_mapper.Map)
            .ToList()!;
    }
    
}