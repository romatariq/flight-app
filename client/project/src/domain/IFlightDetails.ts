import { IAircraft } from "./IAircraft";
import { IFlight } from "./IFlight";

export interface IFlightDetails extends IFlight {
    estimatedDepartureLocal: Date,
    estimatedArrivalLocal: Date,
    
    estimatedDepartureUtc: Date,
    estimatedArrivalUtc: Date,

    departureTerminal: string | null,
    arrivalTerminal: string | null,
    
    departureGate: string | null,
    arrivalGate: string | null,
    
    aircraft: IAircraft | null,
    
    displayDepartureTerminal: boolean,
    displayArrivalTerminal: boolean,
    
    displayDepartureGate: boolean,
    displayArrivalGate: boolean,
    
    percentageOfFlightDone: number,
    userFlightId: string | null,

    flightDistanceKm: number,

    carKgOfCo2PerPerson: number,
    planeKgOfCo2PerPerson: number,
    trainKgOfCo2PerPerson: number,
    shipKgOfCo2PerPerson: number,

    carTravelTimeMinutes: number,
    planeTravelTimeMinutes: number,
    trainTravelTimeMinutes: number,
    shipTravelTimeMinutes: number
}
