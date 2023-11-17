import { type IJWTResponse } from "../dto/IJwtResponse";
import { type IDecodedJwtResponse } from "../dto/IDecodedJwtResponse";
import { EClaimTypes } from "../dto/base/EClaimTypes";
import jwt_decode from "jwt-decode";


export function isJwtValid(jwtResponse: IJWTResponse): boolean {

    const decodedToken: {exp : number} | null =  jwt_decode(jwtResponse.jwt);
    if (decodedToken === null) return false;
    
    const secondsTillExpiration = decodedToken.exp - Math.floor(Date.now() / 1000);
    if (secondsTillExpiration > 5) return true;
    return false;
}


export function getUserId(jwtResponse: IJWTResponse | null): string | undefined {
    if (!jwtResponse) return undefined;
    const decodedToken: IDecodedJwtResponse | undefined = jwt_decode(jwtResponse.jwt);

    if (!decodedToken) {
        return undefined;
    }

    return decodedToken[EClaimTypes.nameIdentifier];
}

export function getUserName(jwtResponse: IJWTResponse | null): string | undefined {
    if (!jwtResponse) return undefined;
    const decodedToken: IDecodedJwtResponse | undefined = jwt_decode(jwtResponse.jwt);

    if (!decodedToken) {
        return undefined;
    }

    return decodedToken[EClaimTypes.firstName] + " " + decodedToken[EClaimTypes.lastName];
}


export function isUserAdmin(jwtResponse: IJWTResponse): boolean {
    const decodedToken: IDecodedJwtResponse | undefined =  jwt_decode(jwtResponse.jwt);

    if (!decodedToken) return false;

    return (decodedToken[EClaimTypes.role] ?? [])
        .includes("admin");
}