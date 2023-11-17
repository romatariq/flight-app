import { EClaimTypes } from "./base/EClaimTypes";

export interface IDecodedJwtResponse {
    [EClaimTypes.nameIdentifier]?: string;
    [EClaimTypes.name]?: string;
    [EClaimTypes.email]?: string;
    [EClaimTypes.securityStamp]?: string;
    [EClaimTypes.firstName]?: string;
    [EClaimTypes.lastName]?: string;
    [EClaimTypes.exp]?: number;
    [EClaimTypes.iss]?: string;
    [EClaimTypes.aud]?: string;
    [EClaimTypes.role]?: string[];
}