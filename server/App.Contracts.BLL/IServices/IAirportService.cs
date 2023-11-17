using App.Contracts.DAL.IRepositories;
using Base.Contracts.DAL;
using Bll = App.Private.DTO.BLL;

namespace App.Contracts.BLL.IServices;

public interface IAirportService : IBaseRepository<Bll.Airport>, IAirportRepositoryCustom<Bll.Airport, Bll.AirportStatistics>
{
    // add your custom service methods here
}
