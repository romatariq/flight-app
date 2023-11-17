# Migrations
~~~bash
# install or update tools
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

# create migration
dotnet ef migrations add Initial --project App.DAL.EF --startup-project WebApp --context AppDbContext

# apply migration
dotnet ef database update --project App.DAL.EF --startup-project WebApp --context AppDbContext
~~~


# Generate controllers

Nuget packages
- Microsoft.VisualStudio.Web.CodeGeneration.Design
- Microsoft.EntityFrameworkCode.SqlServer
- Microsoft.EntityFrameworkCore.Design


~~~bash
# install or update tools
dotnet tool install --global dotnet-aspnet-codegenerator
dotnet tool update --global dotnet-aspnet-codegenerator

cd WebApp

# MVC
dotnet aspnet-codegenerator controller -m Aircraft                -name AircraftsController                -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m AircraftModel           -name AircraftModelsController           -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m Airline                 -name AirlinesController                 -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m Airport                 -name AirportsController                 -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m Country                 -name CountriesController                -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m Flight                  -name FlightsController                  -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m FlightStatus            -name FlightStatusesController           -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m Notification            -name NotificationsController            -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m Recommendation          -name RecommendationsController          -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m RecommendationCategory  -name RecommendationCategoriesController -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m RecommendationReaction  -name RecommendationReactionsController  -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m UserFlight              -name UserFlightsController              -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m UserFlightNotification  -name UserFlightNotificationsController  -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f

dotnet aspnet-codegenerator controller -m AppUser                 -name AppUsersController                 -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f
dotnet aspnet-codegenerator controller -m AppRole                 -name AppRolesController                 -outDir Controllers -dc AppDbContext -udl --referenceScriptLibraries -f


# Rest API
dotnet aspnet-codegenerator controller -m Aircraft                -name AircraftsController                -outDir ApiControllers -api -dc AppDbContext -udl --referenceScriptLibraries -f
~~~

