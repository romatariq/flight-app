import { IUserFlight } from "../domain/IUserFlight";
import { BaseAuthorizedService } from "./BaseAuthorizedService";

export class UserFlightService extends BaseAuthorizedService {
    constructor(){
        super('v1/userFlights/');
    }

    async add(flightId: string): Promise<string | undefined> {
        const jwt = await this.GetValidJwt();
        try {
            const response = await this.axios.post(
                'add',
                {
                    "id": flightId
                },
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                }
            );
            
            if (response.status === 200) {
                return response.data as string;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async delete(userFlightId: string): Promise<true | undefined> {
        const jwt = await this.GetValidJwt();
        try {
            const response = await this.axios.delete(
            'delete', 
            {
                headers: {
                    'Authorization': 'Bearer ' + jwt!.jwt
                },
                params: {
                    'userFlightId': userFlightId
                }
            }
            );
            
            if (response.status === 200) {
                return true;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    
    async getAll(): Promise<IUserFlight[] | undefined> {
        const jwt = await this.GetValidJwt();
        try {
            const response = await this.axios.get(
            'getAll', 
            {
                headers: {
                    'Authorization': 'Bearer ' + jwt!.jwt
                }
            }
            );
            
            if (response.status === 200) {
                const res = response.data as IUserFlight[];
                res.forEach((flight) => {
                    flight.scheduledDepartureLocal = new Date(flight.scheduledDepartureLocal);
                    flight.scheduledArrivalLocal = new Date(flight.scheduledArrivalLocal);
                    flight.scheduledDepartureUtc = new Date(flight.scheduledDepartureUtc);
                    flight.scheduledArrivalUtc = new Date(flight.scheduledArrivalUtc);
                });
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }
    

}
