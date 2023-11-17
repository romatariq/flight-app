using App.Contracts.DAL.IRepositories;
using Base.Contracts.DAL;
using Bll = App.Private.DTO.BLL;

namespace App.Contracts.BLL.IServices;

public interface IFlightService : IBaseRepository<Bll.FlightInfo>, IFlightRepositoryCustom<Bll.FlightInfo, Bll.FlightInfoDetails>
{
    // add your custom service methods here
}