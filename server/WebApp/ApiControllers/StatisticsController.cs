using System.Net;
using System.Net.Mime;
using App.Contracts.BLL;
using App.Mappers.AutoMappers.PublicDTO;
using App.Public.DTO.v1;
using Asp.Versioning;
using AutoMapper;
using Base.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.ApiControllers;

/// <summary>
/// Controller for managing statistics.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class StatisticsController: ControllerBase
{
    private readonly IAppBLL _uow;
    private readonly AirportStatisticsMapper _airportStatsMapper;
    private readonly UserFlightsStatisticsMapper _userStatsMapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="uow">UOW from dependency injection.</param>
    /// <param name="autoMapper">AutoMapper from dependency injection to create needed mapper.</param>
    public StatisticsController(IAppBLL uow, IMapper autoMapper)
    {
        _uow = uow;
        _airportStatsMapper = new AirportStatisticsMapper(autoMapper);
        _userStatsMapper = new UserFlightsStatisticsMapper(autoMapper);
    }
    
    
    /// <summary>
    /// Get airport statistics.
    /// </summary>
    /// <param name="airportIata">IATA (3letter code) of the airport.</param>
    /// <param name="timePeriodHours">Timespan from now to where to get info, default is 24hrs, min is 1hr, max is 1year. 0 is an exception that returns full history.</param>
    /// <returns>Airport statistics.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(AirportStatistics), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AirportStatistics>> AirportStatistics([FromQuery] string airportIata, int timePeriodHours = 24)
    {
        timePeriodHours = Math.Max(0, timePeriodHours);
        timePeriodHours = Math.Min(365*24, timePeriodHours);
        var airportStats = await _uow.AirportService.GetStatistics(airportIata, timePeriodHours);
        if (airportStats == null)
        {
            return BadRequest(new RestApiErrorResponse
            {
                Status = HttpStatusCode.NotFound,
                Error = "Invalid airport"
            });
        }

        return _airportStatsMapper.Map(airportStats)!;
    }
    
    

    /// <summary>
    /// Get user's statistics about his tracked flights.
    /// </summary>
    /// <returns>User flights statistics.</returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(UserFlightsStatistics), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserFlightsStatistics>> UserStatistics()
    {
        return _userStatsMapper.Map(
            await _uow.UserFlightService.GetStatisticsAsync(User.GetUserId())
        )!;
    }    
}