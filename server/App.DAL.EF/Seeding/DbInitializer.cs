using System.Security.Claims;
using App.Domain;
using App.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.FileIO;

namespace App.DAL.EF.Seeding;

public static class DbInitializer
{
    
    private static readonly Guid AdminId = Guid.Parse("bc7458ac-cbb0-4ecd-be79-d5abf19f8c77");
    
    
    public static void InitializeDb(AppDbContext ctx, string webRootPath)
    {
        List<Country>? countries = null;
        if (!ctx.Countries.Any())
        {
            countries = InitializeCountries(webRootPath);
            ctx.Countries.AddRange(countries);
        }
        if (!ctx.Airports.Any())
        {
            countries ??= ctx.Countries.ToList();
            var airports = InitializeAirports(webRootPath, countries);
            ctx.Airports.AddRange(airports);
        }
        if (!ctx.FlightStatuses.Any())
        {
            var flightStatuses = InitializeFlightStatuses();
            ctx.FlightStatuses.AddRange(flightStatuses);
        }
        if (!ctx.AircraftModels.Any())
        {
            var aircraftModels = InitializeAircraftModels();
            ctx.AircraftModels.AddRange(aircraftModels);
        }
        if (!ctx.Airlines.Any())
        {
            var airlines = InitializeAirlines();
            ctx.Airlines.AddRange(airlines);
        }
        if (!ctx.RecommendationCategories.Any())
        {
            var recommendationCategories = InitializeRecommendationCategories();
            ctx.RecommendationCategories.AddRange(recommendationCategories);
        }
        if (!ctx.Notifications.Any())
        {
            var notifications = InitializeNotifications();
            ctx.Notifications.AddRange(notifications);
        }
        ctx.SaveChanges();
    }
    
    
    public static void InitializeTestDb(AppDbContext ctx)
    {
        List<Country>? countries = null;
        if (!ctx.Countries.Any())
        {
            countries = InitializeTestCountries();
            ctx.Countries.AddRange(countries);
        }
        if (!ctx.Airports.Any())
        {
            countries ??= ctx.Countries.ToList();
            var airports = InitializeTestAirports(countries);
            ctx.Airports.AddRange(airports);
        }
        if (!ctx.RecommendationCategories.Any())
        {
            var recommendationCategories = InitializeRecommendationCategories();
            ctx.RecommendationCategories.AddRange(recommendationCategories);
        }
        ctx.SaveChanges();
    }

    public static void MigrateDatabase(AppDbContext ctx)
    {
        ctx.Database.Migrate();
    }

    public static void DropDatabase(AppDbContext ctx)
    {
        ctx.Database.EnsureDeleted();
    }
    
    private static void SeedRoles(RoleManager<AppRole> roleManager)
    {
        if (roleManager.Roles.Any()) return;
        var roles = new List<AppRole>
        {
            new() { Name = "admin" }
        };
        foreach (var role in roles)
        {
            roleManager.CreateAsync(role).Wait();
        }
    }
    
    public static void SeedIdentity(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        SeedRoles(roleManager);
        (Guid id, string email, string password) userData = (AdminId, "admin@app.com", "Foo.bar1");
        var user = userManager.FindByEmailAsync(userData.email).Result;
        if (user != null) return;
        
        user = new AppUser()
        {
            Id = userData.id,
            Email = userData.email,
            UserName = userData.email,
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "App",
            IsVerified = true
        };
        var result = userManager.CreateAsync(user, userData.password).Result;
        if (!result.Succeeded)
        {
            throw new ApplicationException($"Cannot seed users, {result}");
        }
        
        var roleAddResult = userManager.AddToRoleAsync(user, "admin").Result;
        if (!roleAddResult.Succeeded)
        {
            throw new ApplicationException($"Cannot add role to admin, {result}");
        }
        
        userManager.AddClaimsAsync(user, new List<Claim>()
        {
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName)
        }).Wait();
    }

    private static string GetCsvString(string url)
    {
        using var client = new HttpClient();
        return client
            .GetStringAsync(url)
            .Result;
    }
    
    private static IEnumerable<FlightStatus> InitializeFlightStatuses()
    {
        return new List<FlightStatus>()
        {
            new() { Name = "Scheduled" },
            new() { Name = "Live" },
            new() { Name = "Finished" },
            new() { Name = "Cancelled" },
            new() { Name = "Unknown" }
        };
    }

    private static IEnumerable<AircraftModel> InitializeAircraftModels()
    {
        return new List<AircraftModel>()
        {
            new()
            {
                ModelName = "Unknown",
                ModelCode = "Unknown",
            }
        };
    }
    private static IEnumerable<Airline> InitializeAirlines()
    {
        return new List<Airline>()
        {
            new()
            {
                Name = "Unknown",
                Iata = "",
                Icao = ""
            }
        };
    }

    private static IEnumerable<RecommendationCategory> InitializeRecommendationCategories()
    {
        return new List<RecommendationCategory>()
        {
            new() { Category = "Other" },
            new() { Category = "Food" }
        };
    }
    
    private static IEnumerable<Notification> InitializeNotifications()
    {
        return new List<Notification>()
        {
            new() { NotificationType = "Scheduled departure" },
            new() { NotificationType = "Scheduled arrival" },
            new() { NotificationType = "Estimated departure" },
            new() { NotificationType = "Estimated arrival" },
        };
    }

    private static List<Country> InitializeCountries(string webRootPath)
    {
        var filePath = Path.Combine(webRootPath, "countries.csv");
        using var sr = new StreamReader(filePath);
        using var parser = new TextFieldParser(sr);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        // skip headers
        parser.ReadFields();
        var countries = new List<Country>();
        while (!parser.EndOfData) 
        {
            var fields = parser.ReadFields()!;
            
            countries.Add(
                new Country()
                {
                    Name = fields[0],
                    Iso2 = fields[1].ToUpper(),
                    Iso3 = fields[2].ToUpper()
                }
            );
        }
        return countries;
    }

    private static IEnumerable<Airport> InitializeAirports(string webRootPath, List<Country> countries)
    {
        var filePath = Path.Combine(webRootPath, "airports.csv");
        using var sr = new StreamReader(filePath);
        using var parser = new TextFieldParser(sr);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        // skip headers
        parser.ReadFields();
        var airports = new List<Airport>();

        while (!parser.EndOfData) 
        {
            var fields = parser.ReadFields()!;

            var country = countries.FirstOrDefault(c => c.Iso2 == fields[0].ToUpper());

            if (country == null)
            {
                throw new Exception("Country does not exist!" + " iso2:" + fields[0]);
            }

            var airport = new Airport()
            {
                Iata = fields[2].ToUpper(),
                Name = fields[4],
                Country = country,
                DisplayGate = false,
                DisplayTerminal = false,
                Latitude = double.Parse(fields[5]),
                Longitude = double.Parse(fields[6]),
                DisplayAirport = fields[2].ToUpper() == "TLL" || fields[2].ToUpper() == "HEL"
            };
            if (airport.Iata.Trim() == "" || airport.Name.Trim() == "") continue;
            airports.Add(airport);
        }

        return airports;
    }

    private static IEnumerable<Airport> InitializeTestAirports(List<Country> countries)
    {
        return new List<Airport>()
        {
            new()
            {
                Country = countries.First(c => c.Iso2 == "EE"),
                Iata = "TLL",
                Name = "Tallinn Airport",
                DisplayAirport = true,
                Latitude = 59.41329956,
                Longitude = 24.83279991,
            },
            new()
            {
                Country = countries.First(c => c.Iso2 == "FI"),
                Iata = "HEL",
                Name = "Helsinki Airport",
                DisplayAirport = true,
                Latitude = 60.31719971,
                Longitude = 24.9633007,
            },
            new()
            {
                Country = countries.First(c => c.Iso2 == "DE"),
                Iata = "BER",
                Name = "Berlin Airport",
                Latitude = 52.35138889,
                Longitude = 13.49388889,
            }
        };
    }

    private static List<Country> InitializeTestCountries()
    {
        return new List<Country>()
        {
            new()
            {
                Iso2 = "EE",
                Iso3 = "EST",
                Name = "Estonia"
            },
            new()
            {
                Iso2 = "FI",
                Iso3 = "FIN",
                Name = "Finland"
            },
            new()
            {
                Iso2 = "DE",
                Iso3 = "DEU",
                Name = "Germany"
            }
        };
    }

}