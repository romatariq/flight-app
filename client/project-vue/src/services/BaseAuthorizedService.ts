import { type IJWTResponse } from "@/dto/IJwtResponse";
import { BaseService } from "./BaseService";
import { IdentityService } from "./IdentityService";

export abstract class BaseAuthorizedService extends BaseService {


    jwtResponse: IJWTResponse | null = null;
    setJwtResponse: ((data: IJWTResponse | null) => void) | null = null;
    identityService: IdentityService = new IdentityService();

    async GetValidJwt(): Promise<IJWTResponse | null> {
        return await this.identityService.GetOrCreateAndSetJwt(this.jwtResponse, this.setJwtResponse);
    }

    initJwtResponse(jwtResponse: IJWTResponse | null, setJwtResponse: ((data: IJWTResponse | null) => void) | null){
        this.jwtResponse = jwtResponse;
        this.setJwtResponse = setJwtResponse;
    }
}
