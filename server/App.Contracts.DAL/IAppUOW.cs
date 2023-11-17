using App.Contracts.DAL.IRepositories;
using Base.Contracts.DAL;

namespace App.Contracts.DAL;

public interface IAppUOW : IBaseUOW
{
    IAirportRepository AirportRepository { get; }
    IFlightRepository FlightRepository { get; }
    IRecommendationRepository RecommendationRepository { get; }
    IRecommendationCategoryRepository RecommendationCategoryRepository { get; }
    IRecommendationReactionRepository RecommendationReactionRepository { get; }
    IUserFlightRepository UserFlightRepository { get; }
    IUserFlightNotificationRepository UserFlightNotificationRepository { get; }
}
