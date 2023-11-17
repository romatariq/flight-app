using App.Contracts.DAL.IRepositories;
using Base.Contracts.DAL;
using Bll = App.Private.DTO.BLL;

namespace App.Contracts.BLL.IServices;

public interface IUserFlightService : IBaseRepository<Bll.UserFlightInfo>, IUserFlightRepositoryCustom<Bll.UserFlightInfo>
{
    // add your custom service methods here
    
    Task<Bll.UserFlightsStatistics> GetStatisticsAsync(Guid userId);
}
