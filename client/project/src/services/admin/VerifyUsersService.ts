import { IVerifyUser } from "../../domain/IVerifyUser";
import { BaseAuthorizedService } from "../BaseAuthorizedService";

export class VerifyUsersService extends BaseAuthorizedService {
    constructor(){
        super('v1/admin/UserVerification/');
    }

    async getAll(): Promise<IVerifyUser[] | undefined> {
        const jwt = await this.GetValidJwt();
        try {
            const response = await this.axios.get<IVerifyUser[]>(
                'getAll',
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                }
            );
            
            if (response.status === 200) {
                return response.data as IVerifyUser[];
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async set(user: IVerifyUser): Promise<IVerifyUser | undefined> {
        const jwt = await this.GetValidJwt();
        try {
            const response = await this.axios.put<IVerifyUser>(
                'set',
                user,
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                }
            );
            
            if (response.status === 200) {
                return response.data as IVerifyUser;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }
    

}
