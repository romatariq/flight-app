using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using App.DAL.EF;
using App.Domain.Identity;
using Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using App.Public.DTO.v1;
using App.Public.DTO.v1.Identity;
using Asp.Versioning;

namespace WebApp.ApiControllers.identity;

/// <summary>
/// Service for managing user accounts.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/identity/[controller]/[action]")]
public class AccountController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    private readonly Random _rnd = new();
    private readonly AppDbContext _context;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="signInManager">Sign in manager from dependency injection.</param>
    /// <param name="userManager">User manager from dependency injection.</param>
    /// <param name="configuration">Configuration from dependency injection.</param>
    /// <param name="logger">Logger from dependency injection.</param>
    /// <param name="context">DbContext from dependency injection.</param>
    public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager,
        IConfiguration configuration, ILogger<AccountController> logger, AppDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _context = context;
    }
    
    
    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="registrationData">Info required to register a user.</param>
    /// <returns>Ok if user created.</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType( StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register([FromBody] Register registrationData)
    {
        // is user already registered
        var appUser = await _userManager.FindByEmailAsync(registrationData.Email);
        if (appUser != null)
        {
            _logger.LogWarning("User with email {} is already registered", registrationData.Email);
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = $"User with email {registrationData.Email} is already registered"
            });
        }

        // register user
        var refreshToken = new AppRefreshToken();
        appUser = new AppUser()
        {
            Email = registrationData.Email,
            UserName = registrationData.Email,
            FirstName = registrationData.FirstName,
            LastName = registrationData.LastName,
            AppRefreshTokens = new List<AppRefreshToken>() {refreshToken}
        };
        refreshToken.AppUser = appUser;

        var result = await _userManager.CreateAsync(appUser, registrationData.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = result.Errors.First().Description
            });
        }

        // save into claims also the user full name
        result = await _userManager.AddClaimsAsync(appUser, new List<Claim>()
        {
            new(ClaimTypes.GivenName, appUser.FirstName),
            new(ClaimTypes.Surname, appUser.LastName)
        });

        if (!result.Succeeded)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = result.Errors.First().Description
            });
        }

        return Ok();
    }

    
    /// <summary>
    /// Logs in a user.
    /// </summary>
    /// <param name="loginData">Info required to log in.</param>
    /// <param name="expiresInSeconds">Optional, session length in seconds.</param>
    /// <returns>Jwt and refresh token if successful login.</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(JWTResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JWTResponse>> LogIn([FromBody] Login loginData, [FromQuery] int expiresInSeconds)
    {
        if (expiresInSeconds <= 0) expiresInSeconds = int.MaxValue;

        // verify username
        var appUser = await _userManager.FindByEmailAsync(loginData.Email);
        if (appUser == null)
        {
            _logger.LogWarning("WebApi login failed, email {} not found", loginData.Email);
            await Task.Delay(_rnd.Next(100, 1000));

            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User/Password problem"
            });
        }

        // verify username and password
        var result = await _signInManager.CheckPasswordSignInAsync(appUser, loginData.Password, false);
        if (!result.Succeeded)
        {
            _logger.LogWarning("WebApi login failed, password problem for user {}", loginData.Email);
            await Task.Delay(_rnd.Next(100, 1000));
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User/Password problem"
            });
        }

        if (!appUser.IsVerified)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.Forbidden,
                Error = "User is not verified"
            });
        }

        // get claims based user
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);
        if (claimsPrincipal == null)
        {
            _logger.LogWarning("Could not get ClaimsPrincipal for user {}", loginData.Email);
            await Task.Delay(_rnd.Next(100, 1000));
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User/Password problem"
            });
        }

        appUser.AppRefreshTokens = await _context
            .Entry(appUser)
            .Collection(a => a.AppRefreshTokens!)
            .Query()
            .Where(t => t.AppUserId == appUser.Id)
            .ToListAsync();

        // remove expired tokens
        foreach (var userRefreshToken in appUser.AppRefreshTokens)
        {
            if (
                userRefreshToken.ExpirationUtc < DateTime.UtcNow &&
                (
                    userRefreshToken.PreviousExpirationUtc == null ||
                    userRefreshToken.PreviousExpirationUtc < DateTime.UtcNow
                )
            )
            {
                _context.AppRefreshTokens.Remove(userRefreshToken);
            }
        }

        var refreshToken = new AppRefreshToken
        {
            AppUserId = appUser.Id
        };
        _context.AppRefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();


        // generate jwt
        var jwt = IdentityHelpers.GenerateJwt(
            claimsPrincipal.Claims,
            _configuration["JWT:Key"]!,
            _configuration["JWT:Issuer"]!,
            _configuration["JWT:Audience"]!,
            expiresInSeconds < _configuration.GetValue<int>("JWT:ExpiresInSeconds")
                ? expiresInSeconds
                : _configuration.GetValue<int>("JWT:ExpiresInSeconds")
        );

        return new JWTResponse()
        {
            JWT = jwt,
            RefreshToken = refreshToken.RefreshToken,
        };
    }


    /// <summary>
    /// Gets new valid jwt and refresh token.
    /// </summary>
    /// <param name="refreshTokenModel">Current/old jwt and refresh token.</param>
    /// <param name="expiresInSeconds">Optional, new session length in seconds.</param>
    /// <returns>New jwt and refresh token.</returns>
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(JWTResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JWTResponse>> RefreshToken(
        [FromBody] JWTResponse refreshTokenModel,
        [FromQuery] int expiresInSeconds)
    {
        if (expiresInSeconds <= 0) expiresInSeconds = int.MaxValue;

        JwtSecurityToken jwtToken;
        // get user info from jwt
        try
        {
            jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(refreshTokenModel.JWT);
            if (jwtToken == null)
            {
                return BadRequest(new RestApiErrorResponse()
                {
                    Status = HttpStatusCode.BadRequest,
                    Error = "No token"
                });
            }
        }
        catch (Exception e)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = $"Cant parse the token, {e.Message}"
            });
        }

        if (!IdentityHelpers.ValidateToken(refreshTokenModel.JWT, _configuration["JWT:Key"]!,
                _configuration["JWT:Issuer"]!,
                _configuration["JWT:Audience"]!))
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "JWT validation fail"
            });
        }


        var userEmail = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (userEmail == null)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "No email in jwt"
            });
        }

        // get user and tokens
        var appUser = await _userManager.FindByEmailAsync(userEmail);
        if (appUser == null)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = $"User with email {userEmail} not found"
            });
        }
        

        // load and compare refresh tokens
        var dateNow = DateTime.UtcNow.RemoveKind();
        await _context.Entry(appUser).Collection(u => u.AppRefreshTokens!)
            .Query()
            .Where(x =>
                (x.RefreshToken == refreshTokenModel.RefreshToken && x.ExpirationUtc > dateNow) ||
                (x.PreviousRefreshToken == refreshTokenModel.RefreshToken &&
                 x.PreviousExpirationUtc > dateNow)
            )
            .ToListAsync();

        if (appUser.AppRefreshTokens == null || appUser.AppRefreshTokens.Count == 0)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = $"RefreshTokens collection is {(appUser.AppRefreshTokens == null ? "null" : "0")}"
            });
        }

        if (appUser.AppRefreshTokens.Count != 1)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "More than one valid refresh token found"
            });
        }

        // generate new jwt

        // get claims based user
        var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(appUser);
        if (claimsPrincipal == null)
        {
            _logger.LogWarning("Could not get ClaimsPrincipal for user {}", userEmail);
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User/Password problem"
            });
        }

        // generate jwt
        var jwt = IdentityHelpers.GenerateJwt(
            claimsPrincipal.Claims,
            _configuration["JWT:Key"]!,
            _configuration["JWT:Issuer"]!,
            _configuration["JWT:Audience"]!,
                expiresInSeconds < _configuration.GetValue<int>("JWT:ExpiresInSeconds")
                    ? expiresInSeconds
                    : _configuration.GetValue<int>("JWT:ExpiresInSeconds")
        );

        // make new refresh token, keep old one still valid for some time
        var refreshToken = appUser.AppRefreshTokens.First();
        if (refreshToken.RefreshToken == refreshTokenModel.RefreshToken)
        {
            refreshToken.PreviousRefreshToken = refreshToken.RefreshToken;
            refreshToken.PreviousExpirationUtc = DateTime.UtcNow.AddSeconds(_configuration.GetValue<int>("JWT:ExpiresInSeconds"));

            refreshToken.RefreshToken = Guid.NewGuid().ToString();
            refreshToken.ExpirationUtc = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();
        }

        return new JWTResponse()
        {
            JWT = jwt,
            RefreshToken = refreshToken.RefreshToken,
        };
    }
    
    
    /// <summary>
    /// Logs user out and deletes the refresh token.
    /// </summary>
    /// <param name="logout">Refresh token.</param>
    /// <returns>Ok if log out successful.</returns>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(LogoutResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LogoutResponse>> Logout(
        [FromBody]
        Logout logout)
    {
        // delete the refresh token - so user is kicked out after jwt expiration
        // We do not invalidate the jwt - that would require pipeline modification and checking against db on every request
        // so client can actually continue to use the jwt until it expires (keep the jwt expiration time short ~1 min)

        var userId = User.GetUserId();

        var appUser = await _context.Users
            .Where(u => u.Id == userId)
            .SingleOrDefaultAsync();
        if (appUser == null)
        {
            return BadRequest(new RestApiErrorResponse()
            {
                Status = HttpStatusCode.NotFound,
                Error = "User/Password problem"
            });
        }
        
        await _context.Entry(appUser)
            .Collection(u => u.AppRefreshTokens!)
            .Query()
            .Where(x =>
                (x.RefreshToken == logout.RefreshToken) ||
                (x.PreviousRefreshToken == logout.RefreshToken)
            )
            .ToListAsync();

        foreach (var appRefreshToken in appUser.AppRefreshTokens!)
        {
            _context.AppRefreshTokens.Remove(appRefreshToken);
        }

        var deleteCount = await _context.SaveChangesAsync();

        return new LogoutResponse()
        {
            TokenDeleteCount = deleteCount
        };
    }


}