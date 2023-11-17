import { BaseAuthorizedService } from "../BaseAuthorizedService";

export class NotificationService extends BaseAuthorizedService {
    constructor(){
        super('v1/admin/notifications/');
    }

    async sendTrigger(): Promise<undefined> {
        try {
            const jwt = await this.GetValidJwt();
            const response = await this.axios.post(
                'sendNotifications',
                {},
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                }
            );
            
            if (response.status === 200) {
                return undefined
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }
    

}
