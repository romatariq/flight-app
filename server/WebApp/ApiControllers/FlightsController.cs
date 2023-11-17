using System.Net;
using System.Net.Mime;
using App.Contracts.BLL;
using App.Domain.Identity;
using App.Mappers.AutoMappers.PublicDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Asp.Versioning;
using AutoMapper;
using Base.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PublicV1 = App.Public.DTO.v1;

namespace WebApp.ApiControllers;

/// <summary>
/// Controller for managing flights.
/// </summary>
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class FlightsController : ControllerBase
{
    private readonly IAppBLL _uow;
    private readonly UserManager<AppUser> _userManager;
    private readonly FlightMapper _mapper;
    private readonly FlightDetailsMapper _detailsMapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="uow">UOW from dependency injection.</param>
    /// <param name="userManager">User manager from dependency injection.</param>
    /// <param name="autoMapper">AutoMapper from dependency injection to create needed mapper.</param>
    public FlightsController(IAppBLL uow, UserManager<AppUser> userManager, IMapper autoMapper)
    {
        _uow = uow;
        _userManager = userManager;
        _mapper = new FlightMapper(autoMapper);
        _detailsMapper = new FlightDetailsMapper(autoMapper);
    }

    
    /// <summary>
    /// Gets recent departures from given airport.
    /// </summary>
    /// <param name="airportIata">Airport IATA (3 letter code) from where to get departures.</param>
    /// <returns>Departures from given airport, ordered by newest first.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<PublicV1.Flight>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PublicV1.Flight>>> Departures([FromQuery] string airportIata)
    {
        return await DeparturesOrArrivals(airportIata, true);
    }
    
    
    /// <summary>
    /// Gets recent arrivals to given airport.
    /// </summary>
    /// <param name="airportIata">Airport IATA (3 letter code) from where to get arrivals.</param>
    /// <returns>Arrivals to given airport, ordered by newest first.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<PublicV1.Flight>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<PublicV1.Flight>>> Arrivals([FromQuery] string airportIata)
    {
        return await DeparturesOrArrivals(airportIata, false);
    }


    /// <summary>
    /// Gets detailed information about a flight.
    /// </summary>
    /// <param name="flightId">Requested flight id.</param>
    /// <returns>Detailed information about a flight.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PublicV1.FlightDetails), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PublicV1.FlightDetails>> Flight([FromQuery] Guid flightId)
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
        
        var flight = await _uow.FlightService.GetFlightAsync(flightId, appUser.Id);
        var mappedFlight = _detailsMapper.Map(flight);
        if (mappedFlight == null || flight == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Flight not found"
            });
        }
        return mappedFlight;
    }


    private async Task<ActionResult<IEnumerable<PublicV1.Flight>>> DeparturesOrArrivals(string airportIata, bool isDeparture)
    {
        var airport = await _uow.AirportService.GetByIata(airportIata);
        if (airport == null || !airport.DisplayFlights)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = airport == null ? "Airport not found" : "Airport not displayed"
            });
        }
        
        var flights = isDeparture ?
            await _uow.FlightService.GetDepartures(airport.Iata) :
            await _uow.FlightService.GetArrivals(airport.Iata);
        
        return flights.Select(_mapper.Map)
            .ToList()!;
    }

}