import { IBaseEntity } from "./IBaseEntity";

export interface IAirport extends IBaseEntity {
    name: string,
    iata: string,
    displayFlights: boolean,
    countryName: string,
    countryIso2: string,
    countryIso3: string
}
