using App.DAL.EF.Repositories;
using App.Contracts.DAL;
using App.Contracts.DAL.IRepositories;
using Base.DAL.EF;
using FlightInfoCollector;

namespace App.DAL.EF;

public class AppUOW : EFBaseUOW<AppDbContext>, IAppUOW
{

    public AppUOW(AppDbContext context) : base(context)
    {
    }

    private IAirportRepository? _airportRepository;
    private IFlightRepository? _flightRepository;
    private IRecommendationRepository? _recommendationRepository;
    private IRecommendationCategoryRepository? _recommendationCategoryRepository;
    private IRecommendationReactionRepository? _recommendationReactionRepository;
    private IUserFlightRepository? _userFlightRepository;
    private IUserFlightNotificationRepository? _userFlightNotificationRepository;
    
 
    public IAirportRepository AirportRepository =>
        _airportRepository ??= new AirportRepository(DbContext);
 
    public IFlightRepository FlightRepository =>
        _flightRepository ??= new FlightRepository(DbContext);

    public IRecommendationRepository RecommendationRepository =>
        _recommendationRepository ??= new RecommendationRepository(DbContext);
 
    public IRecommendationCategoryRepository RecommendationCategoryRepository =>
        _recommendationCategoryRepository ??= new RecommendationCategoryRepository(DbContext);
 
    public IRecommendationReactionRepository RecommendationReactionRepository =>
        _recommendationReactionRepository ??= new RecommendationReactionRepository(DbContext);
 
    public IUserFlightRepository UserFlightRepository =>
        _userFlightRepository ??= new UserFlightRepository(DbContext);
 
    public IUserFlightNotificationRepository UserFlightNotificationRepository =>
        _userFlightNotificationRepository ??= new UserFlightNotificationRepository(DbContext);
}
