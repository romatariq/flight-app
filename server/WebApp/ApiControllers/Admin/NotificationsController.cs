using System.Net.Mime;
using App.Contracts.BLL;
using App.Public.DTO.v1;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers.Admin;

/// <summary>
/// Service for managing triggers for sending notifications by admin.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = "admin")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/[controller]/[action]")]
public class NotificationsController : ControllerBase
{
    private readonly IAppBLL _uow;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="uow">UOW from dependency injection.</param>
    public NotificationsController(IAppBLL uow)
    {
        _uow = uow;
    }
    
    
    /// <summary>
    /// Trigger for checking if any notifications should be sent.
    /// </summary>
    /// <returns>Nothing.</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult> SendNotifications()
    {
        await _uow.UserFlightNotificationService.SendNotifications();
        return Ok();
    }
}