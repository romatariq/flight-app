import { Link } from "react-router-dom";
import IdentityHeader from "./IdentityHeader";
import { JwtContext } from "../routes/Root";
import { useContext } from "react";
import { isUserAdmin } from "../helpers/IdentityHelpers";

const Header = () => {
    const { jwtResponse } = useContext(JwtContext);

    return (
        <header>
            <nav className="navbar navbar-expand-sm navbar-toggleable-sm border-bottom box-shadow mb-3">
                <div className="container">
                    <Link className="navbar-brand" to="/">FlightApp</Link>
                    <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target=".navbar-collapse" aria-controls="navbarSupportedContent"
                        aria-expanded="false" aria-label="Toggle navigation">
                        <span className="navbar-toggler-icon"></span>
                    </button>
                    <div className="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                        <ul className="navbar-nav flex-grow-1">                           
                            <li className="nav-item">
                                <Link to="/airports" className="nav-link text-dark">Airports</Link>
                            </li>
                            {jwtResponse && <li className="nav-item">
                                <Link to="/flights" className="nav-link text-dark">My Flights</Link>
                            </li>}
                            {(jwtResponse && isUserAdmin(jwtResponse)) && <li className="nav-item">
                                <Link to="/admin/users" className="nav-link text-dark">Verify Users</Link>
                            </li>}
                        </ul>
                        <ul className="navbar-nav">
                            <IdentityHeader/>
                        </ul>
                    </div>
                </div>
            </nav>
        </header>
    );
}

export default Header;
