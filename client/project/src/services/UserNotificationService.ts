import { IUserFlightWithNotifications } from "../domain/IUserFlightWithNotifications";
import { IUserNotification } from "../domain/IUserNotification";
import { IMinimalUserFlightNotification } from "../dto/IMinimalUserFlightNotification";
import { BaseAuthorizedService } from "./BaseAuthorizedService";

export class UserNotificationService extends BaseAuthorizedService {
    constructor(){
        super('v1/userNotifications/');
    }

    async getUserFlightWithNotifications(flightId: string): Promise<IUserFlightWithNotifications | undefined> {
        const jwt = await this.GetValidJwt();
        try {
            const response = await this.axios.get(
                'getUserFlight',
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                    params: {
                        flightId
                    }
                }
            );
            
            if (response.status === 200) {
                return response.data as IUserFlightWithNotifications;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async addNotification(data: IMinimalUserFlightNotification): Promise<IUserNotification | undefined> {
        const jwt = await this.GetValidJwt();
        try {
            const response = await this.axios.post(
                'add',
                data,
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    }
                }
            );
            
            if (response.status === 200) {
                return response.data as IUserNotification;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async deleteNotification(id: string): Promise<true | undefined> {
        const jwt = await this.GetValidJwt();
        try {
            const response = await this.axios.delete(
                'delete',
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                    params: {
                        "userFlightNotificationId": id
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

    

}
