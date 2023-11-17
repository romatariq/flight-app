using System.Net;
using System.Net.Mime;
using App.Contracts.BLL;
using App.Mappers.AutoMappers.PublicDTO;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using AutoMapper;
using PublicV1 = App.Public.DTO.v1;

namespace WebApp.ApiControllers;

/// <summary>
/// Service for getting airports.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]/[action]")]
public class AirportsController: ControllerBase
{
    private readonly IAppBLL _uow;
    private readonly AirportMapper _airportMapper;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="uow">UOW from dependency injection.</param>
    /// <param name="autoMapper">AutoMapper from dependency injection to create needed mapper.</param>
    public AirportsController(IAppBLL uow, IMapper autoMapper)
    {
        _uow = uow;
        _airportMapper = new AirportMapper(autoMapper);
    }
    
    
    /// <summary>
    /// Gets all (limit 25) airports that can be filtered.
    /// </summary>
    /// <param name="filter">Optional, filters by airport iata/name, country iso2/iso3/name.</param>
    /// <returns>Up to 25 airports that best fit the filter.</returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(IEnumerable<PublicV1.Airport>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<PublicV1.Airport>> Airports([FromQuery] string? filter)
    {
        return _uow.AirportService.GetAll(filter)
            .Select(_airportMapper.Map)
            .ToList()!;
    }    
    
    
    /// <summary>
    /// Gets an airport by its iata code.
    /// </summary>
    /// <param name="airportIata">Airport IATA (3 letter code).</param>
    /// <returns>Airport info as <see cref="App.Public.DTO.v1.Airport"/></returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PublicV1.Airport), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PublicV1.RestApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PublicV1.Airport>> Airport([FromQuery] string airportIata)
    {
        var airport = await _uow.AirportService.GetRestDtoByIata(airportIata);
        if (airport == null)
        {
            return BadRequest(new PublicV1.RestApiErrorResponse()
            {
                Status = HttpStatusCode.BadRequest,
                Error = "Airport not found"
            });
        }
        
        return _airportMapper.Map(airport)!;
    }
}