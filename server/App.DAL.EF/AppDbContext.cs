using App.Domain;
using App.Domain.Identity;
using Base.Helpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace App.DAL.EF;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public DbSet<Aircraft> Aircrafts { get; set; } = default!;
    public DbSet<AircraftModel> AircraftModels { get; set; } = default!;
    public DbSet<Airline> Airlines { get; set; } = default!;
    public DbSet<Airport> Airports { get; set; } = default!;
    public DbSet<Country> Countries { get; set; } = default!;
    public DbSet<Flight> Flights { get; set; } = default!;
    public DbSet<FlightStatus> FlightStatuses { get; set; } = default!;
    public DbSet<Notification> Notifications { get; set; } = default!;
    public DbSet<Recommendation> Recommendations { get; set; } = default!;
    public DbSet<RecommendationCategory> RecommendationCategories { get; set; } = default!;
    public DbSet<RecommendationReaction> RecommendationReactions { get; set; } = default!;
    public DbSet<UserFlight> UserFlights { get; set; } = default!;
    public DbSet<UserFlightNotification> UserFlightNotifications { get; set; } = default!;

    public DbSet<AppUser> AppUsers { get; set; } = default!;
    public DbSet<AppRole> AppRoles { get; set; } = default!;
    public DbSet<AppRefreshToken> AppRefreshTokens { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // loop through all entities and all columns and set datetime columns from type "timestamp with time zone" to be of type "timestamp without time zone"
        // alternative1 - in WebApp.program.cs: AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        // alternative2 - in Domain specify with data annotations: [Column(TypeName = "timestamp without time zone")]
        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.RemoveKind(),
            v => v);
        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v == null ? null : v.Value.RemoveKind(),
            v => v);
        
        foreach (var pb in builder.Model
                     .GetEntityTypes()
                     .SelectMany(t => t.GetProperties())
                     .Where(p => p.ClrType == typeof(DateTime) || p.ClrType == typeof(DateTime?))
                     .Select(p => builder.Entity(p.DeclaringEntityType.ClrType).Property(p.Name)))
        {
            pb.HasColumnType("timestamp without time zone");
            if (pb.Metadata.ClrType == typeof(DateTime))
            {
                pb.HasConversion(dateTimeConverter);
            }
            else if (pb.Metadata.ClrType == typeof(DateTime?))
            {
                pb.HasConversion(nullableDateTimeConverter);
            }
        }
        
    }
}
