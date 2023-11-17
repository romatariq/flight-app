import { IAirportStatistics } from "../domain/IAirportStatistics";
import { IUserFlightsStatistics } from "../domain/IUserFlightsStatistics";
import { BaseAuthorizedService } from "./BaseAuthorizedService";

export class StatisticsService extends BaseAuthorizedService {
    constructor(){
        super('v1/statistics/');
    }

    async getAirportStatistics(airportIata: string, timeSpan: number): Promise<IAirportStatistics | undefined> {
        try {
            const response = await this.axios.get(
                'AirportStatistics',
                {
                    params: {
                        'airportIata': airportIata,
                        'timePeriodHours': timeSpan
                    }
                }
            );
            
            if (response.status === 200) {
                const res = response.data as IAirportStatistics;
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }


    async getUserFlightsStatistics(): Promise<IUserFlightsStatistics | undefined> {
        const jwt = await this.GetValidJwt();
        try {
            const response = await this.axios.get(
                'userStatistics', 
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    }
                }
            );
            
            if (response.status === 200) {
                const res = response.data as IUserFlightsStatistics;
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }
    

}
