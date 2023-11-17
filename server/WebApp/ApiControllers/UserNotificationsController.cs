using System.Net;
using System.Net.Mime;
using App.Contracts.BLL;
using App.Domain.Identity;
using App.Mappers.AutoMappers.PublicDTO;
using App.Public.DTO.v1;
using Asp.Versioning;
using AutoMapper;
using Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bll = App.Private.DTO.BLL;

namespace WebApp.ApiControllers;

/// <summary>
/// Controller for managing user's tracked flight notifications.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class UserNotificationsController: ControllerBase
{
    private readonly IAppBLL _uow;
    private readonly UserManager<AppUser> _userManager;
    private readonly UserFlightNotificationMapper _mapper;
    private readonly UserFlightWithNotificationsMapper _detailsMapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="uow">UOW from dependency injection.</param>
    /// <param name="userManager">User manager from dependency injection.</param>
    /// <param name="autoMapper">AutoMapper from dependency injection to create needed mapper.</param>
    public UserNotificationsController(IAppBLL uow, UserManager<AppUser> userManager, IMapper autoMapper)
    {
        _uow = uow;
        _userManager = userManager;
        _mapper = new UserFlightNotificationMapper(autoMapper);
        _detailsMapper = new UserFlightWithNotificationsMapper(autoMapper);
    }
    

    /// <summary>
    /// Delete user's flight notification.
    /// </summary>
    /// <param name="userFlightNotificationId">Identification of the notification to delete.</param>
    /// <returns>Ok if deleted.</returns>
    [HttpDelete]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete([FromQuery] Guid userFlightNotificationId)
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
    
        var res = await _uow.UserFlightNotificationService.DeleteDtoAsync(userFlightNotificationId, appUser);
        if (!res)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "failed to delete"
            });
        }
        await _uow.SaveChangesAsync();
        return Ok();
    }
    
    
    /// <summary>
    /// Add new notification for user's flight.
    /// </summary>
    /// <param name="userFlightNotification">Info required to create a notification.</param>
    /// <returns>Created user flight notification.</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserNotification),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserNotification>> Add([FromBody] MinimalUserFlightNotification userFlightNotification)
    {
        if (userFlightNotification.MinutesFromEvent < -600 || userFlightNotification.MinutesFromEvent > 600)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Invalid minutes"
            });
        }
        
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


        Bll.UserFlightNotificationInfo notification;
        try
        {
            notification = await _uow.UserFlightNotificationService.AddDtoAsync(userFlightNotification.UserFlightId,
                userFlightNotification.MinutesFromEvent, userFlightNotification.NotificationId, appUser);
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = $"{e.Message}"
            });
        }
        await _uow.SaveChangesAsync();
        return _mapper.Map(notification)!;
    }
    
    

    /// <summary>
    /// Get all user's flight notifications with additional info about flight.
    /// </summary>
    /// <param name="flightId">Flight id.</param>
    /// <returns>All user's flight notifications with additional info about flight</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserFlightWithNotifications),StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserFlightWithNotifications>> GetUserFlight([FromQuery] Guid flightId)
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

        var res = await _uow.UserFlightNotificationService.GetUserFlightWithNotifications(flightId, appUser);
        if (res == null)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = $"user not tracking flight"
            });
        }

        return _detailsMapper.Map(res)!;
    }
}