using App.Contracts.BLL.IServices;
using Base.Contracts.BLL;

namespace App.Contracts.BLL;

public interface IAppBLL : IBaseBLL
{
    IAirportService AirportService { get; }
    
    IFlightService FlightService { get;  }
    
    IRecommendationService RecommendationService { get;  }
    
    IRecommendationReactionService RecommendationReactionService { get;  }
    
    IRecommendationCategoryService RecommendationCategoryService { get;  }
    
    IUserFlightService UserFlightService { get;  }
    
    IUserFlightNotificationService UserFlightNotificationService { get;  }
}
