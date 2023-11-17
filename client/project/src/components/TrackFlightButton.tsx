import { JwtContext } from "../routes/Root";
import { useContext, useState } from "react";
import { UserFlightService } from "../services/UserFlightService";


const userFlightService = new UserFlightService();

const UserFlightButton = (props: {flightId: string, userFlightId: string | null}) => {
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const [userFlightId, setUserFlightId] = useState(props.userFlightId as string | null);
    userFlightService.initJwtResponse(jwtResponse, setJwtResponse);

    const handleDelete = async () => {
        if (!userFlightId) return;
        var res = await userFlightService.delete(userFlightId);
        if (res) {
            setUserFlightId(null);
        }
    };

    const handleAdd = async () => {
        if (userFlightId) return;
        var res = await userFlightService.add(props.flightId);
        if (res) {
            setUserFlightId(res);
        }
    };


    if (!jwtResponse) {
        return <></>;
    };
    return (
        <button 
        onClick={userFlightId ? handleDelete : handleAdd}
        className="btn purple">{userFlightId ? "Stop" : "Start"} tracking</button>
    );
}

export default UserFlightButton;