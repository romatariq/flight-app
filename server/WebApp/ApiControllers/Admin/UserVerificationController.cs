using System.Net;
using System.Net.Mime;
using App.Domain.Identity;
using App.Public.DTO.v1;
using App.Public.DTO.v1.Identity;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApp.ApiControllers.Admin;

/// <summary>
/// Service for managing user verification statuses.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Authorize(Roles = "admin")]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/admin/[controller]/[action]")]
public class UserVerificationController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="userManager">User manager from dependency injection.</param>
    public UserVerificationController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    
    
    /// <summary>
    /// Gets all users with their verification statuses, except admins.
    /// </summary>
    /// <returns>All users except admins.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(VerifyUser), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VerifyUser>>> GetAll()
    {
        var admins = await _userManager.GetUsersInRoleAsync("admin");
        return await _userManager.Users
            .OrderByDescending(u => u.CreatedAtUtc)
            .Where(u => !admins.Contains(u))
            .Select(u => new VerifyUser
            {
                Email = u.Email!,
                IsVerified = u.IsVerified
            })
            .ToListAsync();
    }
    
    
    /// <summary>
    /// Changes user verification status.
    /// </summary>
    /// <param name="setUser">The user info to update.</param>
    /// <returns>The updated <see cref="VerifyUser"/>.</returns>
    [HttpPut]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(VerifyUser), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VerifyUser>> Set(
        [FromBody] VerifyUser setUser)
    {
        var user = await _userManager.FindByEmailAsync(setUser.Email);
        if (user == null)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User not found",
            });
        }

        if (user.IsVerified == setUser.IsVerified)
        {
            return setUser;
        }
        
        user.IsVerified = setUser.IsVerified;
        await _userManager.UpdateAsync(user);
        setUser.IsVerified = !setUser.IsVerified;
        return setUser;
    }
}