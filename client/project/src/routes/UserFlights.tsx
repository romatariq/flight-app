import { useContext, useEffect, useState } from "react";
import { JwtContext } from "./Root";
import { Link } from "react-router-dom";
import { UserFlightService } from "../services/UserFlightService";
import { IUserFlight } from "../domain/IUserFlight";
import UserFlightCard from "../components/UserFlightCard";


const userFlightService = new UserFlightService();

const UserFlights = () => {
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const [userFlights, setUserFlights] = useState([] as IUserFlight[]);
    userFlightService.initJwtResponse(jwtResponse, setJwtResponse);


    useEffect(() => {
        let isCancelled = false;
        const fetchUserFlights = async () => {
            const res = await userFlightService.getAll();
            if (res && !isCancelled) {
                setUserFlights(res);
            }
        }

        fetchUserFlights();
        return () => {
            isCancelled = true;
        }
    }, [])



    if (!jwtResponse) {
        return (
        <div className="text-center">
            <h1>You have to be signed in!</h1>
            <Link to={"/login"}>Sign in</Link>
        </div>
        );
    };

    return(
        <div>
            <h1 className="text-center user-flights-header">My Flights</h1>
            <h3>
                <Link to={"/flights/statistics/"}>Statistics</Link>
            </h3>
            <div className="user-flight-card bold">
                <div>Flight</div>
                <div>Departure - Arrival</div>
                <div>Departure UTC</div>
                <div>Arrival UTC</div>
            </div>
            {userFlights
            .sort((a: IUserFlight, b: IUserFlight) => {
                return b.scheduledDepartureUtc.getTime() - a.scheduledDepartureUtc.getTime();
            })
            .map((userFlight) => {
                return <UserFlightCard userFlight={userFlight} key={userFlight.id} />
            })
            }
        </div>
    )
}

export default UserFlights;