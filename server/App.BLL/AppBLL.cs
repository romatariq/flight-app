using App.BLL.Services;
using App.Contracts.BLL;
using App.Contracts.BLL.IServices;
using App.Contracts.DAL;
using AutoMapper;
using Base.BLL;
using FlightInfoCollector;

namespace App.BLL;

public class AppBLL : BaseBLL<IAppUOW>, IAppBLL
{
    private readonly AutoMapper.IMapper _mapper;
    private readonly IDataCollector _dataCollector;



    public AppBLL(IAppUOW uow, IMapper mapper, IDataCollector dataCollector) : base(uow)
    {
        _mapper = mapper;
        _dataCollector = dataCollector;
    }


    private IAirportService? _airportService;
    
    private IFlightService? _flightService;
    
    private IRecommendationService? _recommendationService;
    
    private IRecommendationReactionService? _recommendationReactionService;
    
    private IRecommendationCategoryService? _recommendationCategoryService;
    
    private IUserFlightService? _userFlightService;
    
    private IUserFlightNotificationService? _userFlightNotificationService;
    
    
    public IAirportService AirportService =>
        _airportService ??= new AirportService(Uow, _mapper);
    
    public IFlightService FlightService =>
        _flightService ??= new FlightService(Uow, _mapper, _dataCollector);
    
    public IRecommendationService RecommendationService =>
        _recommendationService ??= new RecommendationService(Uow, _mapper);
    
    public IRecommendationReactionService RecommendationReactionService =>
        _recommendationReactionService ??= new RecommendationReactionService(Uow, _mapper);
    
    public IRecommendationCategoryService RecommendationCategoryService =>
        _recommendationCategoryService ??= new RecommendationCategoryService(Uow, _mapper);
    
    public IUserFlightService UserFlightService =>
        _userFlightService ??= new UserFlightService(Uow, _mapper);
    
    public IUserFlightNotificationService UserFlightNotificationService =>
        _userFlightNotificationService ??= new UserFlightNotificationService(Uow, _mapper);
}
