using App.Domain.Identity;
using Base.Contracts.DAL;
using Dal = App.Private.DTO.DAL;


namespace App.Contracts.DAL.IRepositories;

public interface IUserFlightNotificationRepository: IBaseRepository<Dal.UserFlightNotificationInfo>, 
    IUserFlightNotificationRepositoryCustom<Dal.UserFlightNotificationInfo, Dal.UserFlightWithNotifications>
{
    Task<List<Dal.UserNotification>> GetAllCurrentNotificationsAsync();

    void DeleteAsync(IEnumerable<Guid> ids);
}


public interface IUserFlightNotificationRepositoryCustom<TEntity, TEntity2>
{
    Task<bool> DeleteDtoAsync(Guid id, AppUser appUser);

    Task<TEntity> AddDtoAsync(Guid userFlightId, int minutesFromEvent, Guid notificationTypeId, AppUser appUser);
    
    Task<TEntity2?> GetUserFlightWithNotifications(Guid flightId, AppUser appUser);
}