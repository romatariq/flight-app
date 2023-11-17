import { useContext } from "react";
import { JwtContext } from "../Root";
import jwt_decode from "jwt-decode";
import { IDecodedJwtResponse } from "../../dto/IDecodedJwtResponse";

const Info = () => {
    const { jwtResponse } = useContext(JwtContext);

    const decodedToken: IDecodedJwtResponse | undefined =  jwtResponse ? jwt_decode(jwtResponse.jwt) : undefined;

    return (
        <>
            <dl className="row">
                <dt className="col-sm-2">
                    jwt decoded
                </dt>
                <dd className="col-sm-10 text-break">
                    <pre>
                        {jwtResponse ? JSON.stringify(jwt_decode(jwtResponse?.jwt), null, 4) : "no jwt"}
                    </pre>
                </dd>

                <dt className="col-sm-2">
                    jwt
                </dt>
                <dd className="col-sm-10 text-break">
                    {jwtResponse?.jwt}
                </dd>

                <dt className="col-sm-2">
                    refreshToken
                </dt>
                <dd className="col-sm-10">
                    {jwtResponse?.refreshToken}
                </dd>

                <dt className="col-sm-2">
                    Seconds till jwt expires
                </dt>
                <dd className="col-sm-10">
                    {decodedToken?.exp ? decodedToken.exp - Math.floor(Date.now() / 1000) : "no jwt"}
                </dd>
            </dl>
        </>
    );
}

export default Info;
