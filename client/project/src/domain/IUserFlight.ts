import { IBaseEntity } from "./IBaseEntity";

export interface IUserFlight extends IBaseEntity {
    flightId: string;
    flightIata: string;
    
    departureAirportIata: string;
    arrivalAirportIata: string;

    scheduledDepartureUtc: Date;
    scheduledArrivalUtc: Date;

    scheduledDepartureLocal: Date;
    scheduledArrivalLocal: Date;
}
