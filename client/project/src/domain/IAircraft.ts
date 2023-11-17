import { IBaseEntity } from "./IBaseEntity";

export interface IAircraft extends IBaseEntity {
    icao: string,
    registration: string | null,
    latitude: string | null,
    longitude: string | null,
    speedKmh: string,
    modelName: string
}
