import { useContext, useEffect, useState } from "react";
import { JwtContext } from "./Root";
import { AirportService } from "../services/AirportService";
import { IAirport } from "../domain/IAirport";
import { Link, useParams } from "react-router-dom";
import { getAirportTitle } from "../helpers/AirportHelpers";
import Error from "../components/Error";
import NavigationLink from "../components/NavigationLink";

const airportService = new AirportService();

const Airport = () => {
    const { jwtResponse } = useContext(JwtContext);
    const { airportIata } = useParams<{airportIata: string}>() ;
    const [airport, setAirport] = useState(null as IAirport | null);

    useEffect(() => {
        let isCancelled = false;
        const fetchAirport = async () => {
            if (!airportIata) return;

            const fetchedAirport = await airportService.getAirport(airportIata);
            if (fetchedAirport && !isCancelled) {
                setAirport(fetchedAirport);
            }
        }
        fetchAirport();
        return () => {
            isCancelled = true;
        }
    }, [airportIata])

    if (!airportIata) {
        return <Error errorMessage="Invalid url"/>;
    }

    if (!airport) {
        return <></>;
    }


    return <div className="text-center">
        <NavigationLink items={[["/airports/", "airports"]]} lastItem={airportIata} />
        <h1>{airport && getAirportTitle(airport.name, airport.iata)}</h1>
        <div>
            {jwtResponse && airport?.displayFlights && 
                <Link to={`/airports/${airportIata.toLowerCase()}/arrivals`} className="btn btn-primary">Arrivals</Link>}
            {jwtResponse && airport?.displayFlights && 
                <Link to={`/airports/${airportIata.toLowerCase()}/departures`} className="btn btn-primary">Departures</Link>}
            {airport?.displayFlights && 
                <Link to={`/airports/${airportIata.toLowerCase()}/statistics`} className="btn btn-primary">Statistics</Link>}
            <Link to={`/airports/${airportIata.toLowerCase()}/reviews`} className="btn btn-primary">Reviews</Link>
        </div>
    </div>

}


export default Airport;
