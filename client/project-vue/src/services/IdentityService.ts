import { type IJWTResponse } from "../dto/IJwtResponse";
import { type ILoginData } from "../dto/ILoginData";
import { type IRegisterData } from "../dto/IRegisterData";
import { type IErrorResponse } from "../dto/IErrorResponse";
import { BaseService } from "./BaseService";
import { isJwtValid } from "../helpers/IdentityHelpers";


export class IdentityService extends BaseService {
    constructor(){
        super('v1/identity/account/');
    }

    async register(data: IRegisterData): Promise<null | undefined> {
        try {
            const response = await this.axios.post('register', data);

            if (response.status === 200) {
                return null;
            }
            return undefined;
        } catch (e){
            return undefined;
        }
    }   
    async login(data: ILoginData): Promise<IJWTResponse | undefined | null> {
        try {
            const response = await this.axios.post<IJWTResponse | IErrorResponse>('login', data);
            
            if (response.status === 200) {
                return response.data as IJWTResponse;
            }
            return undefined;
        } catch (e: any) {
            if (e.name === 'AxiosError' && e.response.data.status === 403 && e.response.data.error === 'User is not verified') {
                return null;
            }
            return undefined;
        }
    }

    async logout(data: IJWTResponse): Promise<true | undefined> {
        
        try {
            const response = await this.axios.post(
                'logout',
                {
                    "refreshToken": data.refreshToken,
                },
                {
                    headers: {
                        'Authorization': 'Bearer ' + data.jwt
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

    async getNewJwt(data: IJWTResponse): Promise<IJWTResponse | undefined> {
        try {
            const response = await this.axios.post(
                'refreshToken', 
                data,
                {
                    headers: {
                        'Authorization': 'Bearer ' + data.jwt
                    }
                }
            );

            if (response.status === 200) {
                return response.data as IJWTResponse;
            }
            return undefined;
        } catch (e) {
            return undefined;
        }
    }

    async GetOrCreateAndSetJwt(
        jwtResponse: IJWTResponse | null, 
        setJwtResponse: ((data: IJWTResponse | null) => void) | null)
        : Promise<IJWTResponse | null> {
            
        if (!jwtResponse || !setJwtResponse) return null;
    
        if (!isJwtValid(jwtResponse)) {
            const newJwt = await this.getNewJwt(jwtResponse) ?? null;
            setJwtResponse(newJwt);
            return newJwt;
        }
        return jwtResponse;
    }

}
