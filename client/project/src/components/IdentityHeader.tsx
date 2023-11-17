import { useContext } from "react";
import { Link, useNavigate } from "react-router-dom";
import { JwtContext } from "../routes/Root";
import { IdentityService } from "../services/IdentityService";
import { getUserName } from "../helpers/IdentityHelpers";


const identityService = new IdentityService();

const IdentityHeader = () => {
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const navigate = useNavigate();

    const logout = () => {
        if (jwtResponse)
            identityService.logout(jwtResponse).then(response => {
                if (setJwtResponse)
                    setJwtResponse(null);
                navigate("/");
            });
    }
    
    if (jwtResponse) {
        return (
            <>
                <li className="nav-item">
                    <Link to="info" className="nav-link text-dark">
                        {getUserName(jwtResponse)}
                    </Link>
                </li>
                <li className="nav-item">
                    <button onClick={(e) => {
                        e.preventDefault();
                        logout();
                    }} className="fake-button nav-link text-dark">Logout</button>
                </li>
            </>
        );
    }
    return (
        <>
            <li className="nav-item">
                <Link to="register" className="nav-link text-dark">Register</Link>
            </li>
            <li className="nav-item">
                <Link to="login" className="nav-link text-dark">Login</Link>
            </li>
        </>
    );
}

export default IdentityHeader;