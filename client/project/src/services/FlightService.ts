import { IFlight } from "../domain/IFlight";
import { IFlightDetails } from "../domain/IFlightDetails";
import { BaseAuthorizedService } from "./BaseAuthorizedService";

export class FlightService extends BaseAuthorizedService {
    constructor(){
        super('v1/flights/');
    }

    async getDepartures(airportIata: string): Promise<IFlight[] | undefined> {
        try {
            const jwt = await this.GetValidJwt();
            const response = await this.axios.get(
                'departures', 
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                    params: {
                        'airportIata': airportIata
                    }
                }
            );
            
            if (response.status === 200) {
                const res = response.data as IFlight[];
                res.forEach((flight) => {
                    flight.scheduledDepartureLocal = new Date(flight.scheduledDepartureLocal);
                    flight.scheduledArrivalLocal = new Date(flight.scheduledArrivalLocal);
                    flight.scheduledDepartureUtc = new Date(flight.scheduledDepartureUtc);
                    flight.scheduledArrivalUtc = new Date(flight.scheduledArrivalUtc);
                })
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async getArrivals(airportIata: string): Promise<IFlight[] | undefined> {
        try {
            const jwt = await this.GetValidJwt();
            const response = await this.axios.get(
                'arrivals', 
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                    params: {
                        'airportIata': airportIata
                    }
                }
            );
            
            if (response.status === 200) {
                const res = response.data as IFlight[];
                res.forEach((flight) => {
                    flight.scheduledDepartureLocal = new Date(flight.scheduledDepartureLocal);
                    flight.scheduledArrivalLocal = new Date(flight.scheduledArrivalLocal);
                    flight.scheduledDepartureUtc = new Date(flight.scheduledDepartureUtc);
                    flight.scheduledArrivalUtc = new Date(flight.scheduledArrivalUtc);
                })
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }
    
    async getFlight(flightId: string): Promise<IFlightDetails | undefined> {
        try {
            const jwt = await this.GetValidJwt();
            const response = await this.axios.get(
                'flight', 
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                    params: {
                        'flightId': flightId
                    }
                }
            );
            
            if (response.status === 200) {
                const res = response.data as IFlightDetails;
                res.scheduledDepartureLocal = new Date(res.scheduledDepartureLocal);
                res.scheduledArrivalLocal = new Date(res.scheduledArrivalLocal);
                res.scheduledDepartureUtc = new Date(res.scheduledDepartureUtc);
                res.scheduledArrivalUtc = new Date(res.scheduledArrivalUtc);
                res.estimatedDepartureLocal = new Date(res.estimatedDepartureLocal);
                res.estimatedArrivalLocal = new Date(res.estimatedArrivalLocal);
                res.estimatedDepartureUtc = new Date(res.estimatedDepartureUtc);
                res.estimatedArrivalUtc = new Date(res.estimatedArrivalUtc);
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }
}
