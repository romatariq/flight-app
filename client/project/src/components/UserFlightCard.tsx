import { Link } from "react-router-dom";
import { IUserFlight } from "../domain/IUserFlight";
import "../styles/userFlights.css"
import { getFullDateTime } from "../helpers/TimeHelpers";
import { UserFlightService } from "../services/UserFlightService";
import { useContext, useState } from "react";
import { JwtContext } from "../routes/Root";


const userFlightService = new UserFlightService();

const UserFlightCard = (props: {userFlight: IUserFlight}) => {
    const {jwtResponse, setJwtResponse} = useContext(JwtContext);
    userFlightService.initJwtResponse(jwtResponse, setJwtResponse);
    const [isDeleted, setIsDeleted] = useState(false);

    const handleDelete =  async () => {
        const res = await userFlightService.delete(props.userFlight.id!);
        if (res) {
            setIsDeleted(true);
        }
    }

    if (isDeleted) {
        return <></>;
    }

    return (
        <div className="user-flight-card">
            <div>
                <Link to={`/flights/flight/${props.userFlight.flightId}`} className="back-link">{props.userFlight.flightIata}</Link>
            </div>
            <div>{props.userFlight.departureAirportIata + " - " + props.userFlight.arrivalAirportIata}</div>
            <div>{getFullDateTime(props.userFlight.scheduledDepartureUtc)}</div>
            <div>{getFullDateTime(props.userFlight.scheduledArrivalUtc)}</div>
            <div>
                <Link to={`/flights/edit/${props.userFlight.flightId}/`} className="user-flight-link">Edit</Link>
                <button
                    onClick={handleDelete} 
                    className="btn btn-danger">Delete</button>
            </div>
        </div>
    )
}

export default UserFlightCard;