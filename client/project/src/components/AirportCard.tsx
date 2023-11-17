import { Link } from "react-router-dom";
import { IAirport } from "../domain/IAirport";
import "../styles/airports.css";

const AirportCard = (props: {airport: IAirport}) => {

    return (
        <div className="card airport text-center">
            <div className="card-body">
                <h5 className="card-title">{props.airport.name} [{props.airport.iata}]</h5>
                <p className="card-text">{`${props.airport.countryName} [${props.airport.countryIso2}/${props.airport.countryIso3}]`}</p>
            </div>
            <Link to={`/airports/${props.airport.iata.toLowerCase()}`}
                className="btn btn-outline-secondary"
                >Show more
            </Link>
        </div>
    )
}

export default AirportCard;
