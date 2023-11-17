import { IAircraft } from "./IAircraft";
import { IBaseEntity } from "./IBaseEntity";

export interface IFlight extends IBaseEntity {
    flightIata: string,

    scheduledDepartureLocal: Date,
    scheduledArrivalLocal: Date,
    
    scheduledDepartureUtc: Date,
    scheduledArrivalUtc: Date,
    
    airline: string,
    
    departureAirportIata: string,
    arrivalAirportIata: string,
    
    departureAirportName: string,
    arrivalAirportName: string,
    
    status: string
}
