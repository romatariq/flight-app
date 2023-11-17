using AutoMapper;
using Base.DAL;
using Bll = App.Private.DTO.BLL;
using Dal = App.Private.DTO.DAL;
namespace App.Mappers.AutoMappers.BLL;


public class FlightDetailsMapper: BaseMapper<Dal.FlightInfoDetails, Bll.FlightInfoDetails>
{
    public FlightDetailsMapper(IMapper mapper) : base(mapper)
    {
    }

}