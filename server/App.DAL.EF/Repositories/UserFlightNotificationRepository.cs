using App.Contracts.DAL.IRepositories;
using Base.DAL.EF;
using App.Domain.Identity;
using Base.Helpers;
using Dal = App.Private.DTO.DAL;
using Microsoft.EntityFrameworkCore;

namespace App.DAL.EF.Repositories;

public class UserFlightNotificationRepository: EFBaseRepository<Domain.UserFlightNotification, AppDbContext>, IUserFlightNotificationRepository
{
    public UserFlightNotificationRepository(AppDbContext dbContext) : base(dbContext)
    {
    }


    public async Task<bool> DeleteDtoAsync(Guid id, AppUser appUser)
    {
        var userNotificationDb = await DbSet
            .Include(ufn => ufn.UserFlight)
            .SingleOrDefaultAsync(ufn => 
                ufn.Id == id && ufn.UserFlight!.AppUserId == appUser.Id);
        if (userNotificationDb == null)
        {
            return false;
        }
        
        DbSet.Remove(userNotificationDb);
        return true;
    }

    public async Task<Dal.UserFlightNotificationInfo> AddDtoAsync(Guid userFlightId, int minutesFromEvent, Guid notificationTypeId, AppUser appUser)
    {
        var userNotificationDb = await DbSet
            .FirstOrDefaultAsync(ufn =>
                ufn.NotificationId == notificationTypeId &&
                ufn.MinutesFromEvent == minutesFromEvent &&
                ufn.UserFlightId == userFlightId);
        if (userNotificationDb != null)
        {
            throw new Exception("User notification already exists");
        }
        
        var notificationType = await DbContext.Notifications
            .FindAsync(notificationTypeId);
        if (notificationType == null)
        {
            throw new Exception("Notification type does not exist");
        }
        
        var userFlight = await DbContext.UserFlights
            .FindAsync(userFlightId);
        if (userFlight == null || userFlight.AppUserId != appUser.Id)
        {
            throw new Exception("Problem with user flight");
        }
        
        var notification = new Domain.UserFlightNotification
        {
            MinutesFromEvent = minutesFromEvent,
            UserFlightId = userFlightId,
            NotificationId = notificationTypeId
        };
        var res = (await DbSet.AddAsync(notification)).Entity;

        return new Dal.UserFlightNotificationInfo()
        {
            Id = res.Id,
            UserId = userFlight.AppUserId,
            NotificationId = notification.NotificationId,
            NotificationType = notificationType.NotificationType,
            MinutesFromEvent = notification.MinutesFromEvent
        };
    }

    public async Task<Dal.UserFlightWithNotifications?> GetUserFlightWithNotifications(Guid flightId, AppUser appUser)
    {
        return await DbContext.UserFlights
            .Include(uf => uf.Flight)
                .ThenInclude(f => f!.DepartureAirport)
            .Include(uf => uf.Flight)
                .ThenInclude(f => f!.ArrivalAirport)
            .Include(uf => uf.UserFlightNotifications!)
                .ThenInclude(ufn => ufn.Notification)
            .Where(uf => 
                uf.Flight!.Id == flightId && uf.AppUserId == appUser.Id)
            .Select(uf => new Dal.UserFlightWithNotifications()
            {
                Id = uf.Id,
                FlightIata = uf.Flight!.FlightIata,
                DepartureAirportIata = uf.Flight.DepartureAirport!.Iata,
                DepartureAirportName = uf.Flight.DepartureAirport!.Name,
                ArrivalAirportIata = uf.Flight.ArrivalAirport!.Iata,
                ArrivalAirportName = uf.Flight.ArrivalAirport!.Name,
                AllNotificationTypes = DbContext.Notifications
                    .Select(n => new Dal.Notification
                    {
                        Id = n.Id,
                        Type = n.NotificationType
                    }).ToList(),
                UserNotifications = uf.UserFlightNotifications!
                    .Select(ufn => new Dal.UserFlightNotificationInfo()
                    {
                        Id = ufn.Id,
                        UserId = uf.AppUserId,
                        NotificationId = ufn.NotificationId,
                        NotificationType = ufn.Notification!.NotificationType,
                        MinutesFromEvent = ufn.MinutesFromEvent
                    })
                    .ToList()
            })
            .SingleOrDefaultAsync();
    }

    public async Task<List<Dal.UserNotification>> GetAllCurrentNotificationsAsync()
    {
        var dateNow = DateTime.UtcNow.RemoveKind();

        return await DbSet
            .Include(ufn => ufn.Notification)
            .Include(ufn => ufn.UserFlight)
                .ThenInclude(uf => uf!.Flight)
                    .ThenInclude(f => f!.DepartureAirport)
            .Include(ufn => ufn.UserFlight)
                .ThenInclude(uf => uf!.Flight)
                    .ThenInclude(f => f!.ArrivalAirport)
            .Include(ufn => ufn.UserFlight)
                .ThenInclude(uf => uf!.AppUser)
            .Where(ufn =>
                ufn.Notification!.NotificationType == "Scheduled departure" &&
                ufn.UserFlight!.Flight!.ScheduledDepartureUtc.AddMinutes(ufn.MinutesFromEvent) < dateNow ||
                
                ufn.Notification!.NotificationType == "Scheduled arrival" &&
                ufn.UserFlight!.Flight!.ScheduledArrivalUtc.AddMinutes(ufn.MinutesFromEvent) < dateNow ||
                
                ufn.Notification!.NotificationType == "Estimated departure" &&
                ufn.UserFlight!.Flight!.ExpectedDepartureUtc.AddMinutes(ufn.MinutesFromEvent) < dateNow ||                
                
                ufn.Notification!.NotificationType == "Estimated arrival" &&
                ufn.UserFlight!.Flight!.ExpectedArrivalUtc.AddMinutes(ufn.MinutesFromEvent) < dateNow
            )
            .Select(ufn => new Dal.UserNotification
            {
                Id = ufn.Id,
                UserEmail = ufn.UserFlight!.AppUser!.Email!,
                UserFirstName = ufn.UserFlight!.AppUser!.FirstName,
                NotificationType = ufn.Notification!.NotificationType,
                MinutesFromEvent = ufn.MinutesFromEvent,
                FlightIata = ufn.UserFlight.Flight!.FlightIata,
                DepartureAirportName = ufn.UserFlight.Flight.DepartureAirport!.Name,
                ArrivalAirportName = ufn.UserFlight.Flight.ArrivalAirport!.Name,
            })
            .ToListAsync();
    }

    public void DeleteAsync(IEnumerable<Guid> ids)
    {
        var res = DbContext.UserFlightNotifications
            .Where(ufn => ids.Contains(ufn.Id))
            .AsEnumerable();
        
        DbContext.UserFlightNotifications.RemoveRange(res);
    }
    
}