import { IAirport } from "../domain/IAirport";
import { BaseService } from "./BaseService";

export class AirportService extends BaseService {
    constructor(){
        super('v1/airports/');
    }

    async getAirports(filter: string): Promise<IAirport[] | undefined> {
        try {
            const response = await this.axios.get(
                'airports', 
                {
                    params: {
                        'filter': filter,
                    }
                }
            );
            
            if (response.status === 200) {
                return response.data as IAirport[];
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async getAirport(airportIata: string): Promise<IAirport | undefined> {
        try {
            const response = await this.axios.get(
                'airport', 
                {
                    params: {
                        'airportIata': airportIata,
                    }
                }
            );
            
            if (response.status === 200) {
                return response.data as IAirport;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }
}
