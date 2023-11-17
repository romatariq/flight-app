using App.Contracts.DAL.IRepositories;
using Base.Contracts.DAL;
using Bll = App.Private.DTO.BLL;

namespace App.Contracts.BLL.IServices;

public interface IUserFlightNotificationService : IBaseRepository<Bll.UserFlightNotificationInfo>, 
    IUserFlightNotificationRepositoryCustom<Bll.UserFlightNotificationInfo, Bll.UserFlightWithNotifications>
{
    // add your custom service methods here
    Task SendNotifications();
}
