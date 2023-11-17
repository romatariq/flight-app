import { INameCounter } from "../dto/INameCounter";

export interface IAirportStatistics {
    airportName: string,
    airportIata: string,
    
    departuresCount: number,
    arrivalsCount: number,

    departureAirlines: INameCounter[],
    arrivalAirlines: INameCounter[],

    departureCountries: INameCounter[],
    arrivalCountries: INameCounter[],
}