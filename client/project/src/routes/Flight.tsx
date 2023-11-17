import { useParams } from "react-router-dom";
import { useContext, useEffect, useState } from "react";
import { FlightService } from "../services/FlightService";
import { JwtContext } from "./Root";
import Error from "../components/Error";
import "../styles/flight.css"
import { getTimeWithDay, getTimezoneOffsetMinutes } from "../helpers/TimeHelpers";
import Loading from "../components/Loading";
import { IFlightDetails } from "../domain/IFlightDetails";
import { AirportClockBasic } from "../components/AirportClock";
import FlightHeader from "../components/FlightHeader";
import TrackFlightButton from "../components/TrackFlightButton";
import FlightNavigationLink from "../components/FlightNavigationLink";
import FlightComparisonTable from "../components/FlightComparisonTable";


const flightService =  new FlightService();

const Flight = (props: {isDeparture: boolean}) => {
    
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const [errors, setErrors] = useState([] as string[]);
    const { flightId } = useParams<{flightId: string}>();
    const [flight, setFlight] = useState(null as IFlightDetails | null);
    const {airportIata} = useParams<{airportIata: string}>();
    flightService.initJwtResponse(jwtResponse, setJwtResponse);
    let timeOffSetInMinutesDeparture: number | undefined;
    let timeOffSetInMinutesArrival: number | undefined;

    if (flight) {
        timeOffSetInMinutesDeparture = getTimezoneOffsetMinutes(flight.scheduledDepartureUtc, flight.scheduledDepartureLocal);
        timeOffSetInMinutesArrival = getTimezoneOffsetMinutes(flight.scheduledArrivalUtc, flight.scheduledArrivalLocal);
    }

    useEffect(() => {
        let isCancelled = false;
        const fetchFlight = async () => {

            if (!flightId) {
                setErrors(["Missin url param!"]);
                return;
            }

            const fetchedFlight = await flightService.getFlight(flightId);
            if (fetchedFlight && !isCancelled) {
                setFlight(fetchedFlight);
                setErrors([]);
                return;
            } else if (!isCancelled) {
                setErrors(["Flight not found!"]);
            }
        };
        fetchFlight();

        return () => {
            isCancelled = true;
        }

    }, [flightId]);


    if (errors.length > 0) {
        return <Error errorMessage={errors[0]}/>;
    }
    if (!flight) {
        return <Loading/>;
    }

    return (
        <>
        <FlightNavigationLink airportIata={airportIata} isDeparture={props.isDeparture} flightIata={flight.flightIata} />
        <FlightHeader flight={flight}/>
        <h4 style={{"marginBottom" : "50px"}} className="text-center flight-">{flight.flightIata} - {flight.status.toLowerCase()}</h4>

        <div className="row">
            <div className="col-12 col-md-6">
                <h3>Departure</h3>
                <h4>{"Local time: "}
                    <AirportClockBasic timeOffSetInMinutes={timeOffSetInMinutesDeparture}/>
                </h4>
                <ul>
                    <li>{"Scheduled: " + getTimeWithDay(flight.scheduledDepartureLocal)}</li>
                    <li>{"Estimated: " + getTimeWithDay(flight.estimatedDepartureLocal)}</li>
                    {flight.departureTerminal && <li>Terminal: {flight.departureTerminal}</li>}
                    {flight.departureGate && <li>Gate: {flight.departureGate}</li>}
                </ul>
                <p className="disclaimer">{getDisclaimer(flight.displayDepartureGate, flight.displayDepartureTerminal)}</p>
            </div>
            <div className="col-12 col-md-6">
                <h3>Arrival</h3>
                <h4>{"Local time: "}
                    <AirportClockBasic timeOffSetInMinutes={timeOffSetInMinutesArrival}/>
                </h4>
                <ul>
                    <li>{"Scheduled: " + getTimeWithDay(flight.scheduledArrivalLocal)}</li>
                    <li>{"Estimated: " + getTimeWithDay(flight.estimatedArrivalLocal)}</li>
                    {flight.arrivalTerminal && <li>Terminal: {flight.arrivalTerminal}</li>}
                    {flight.arrivalGate && <li>Gate: {flight.arrivalGate}</li>}
                </ul>
                <p className="disclaimer">{getDisclaimer(flight.displayArrivalGate, flight.displayArrivalTerminal)}</p>
            </div>
        </div>


        <FlightComparisonTable flight={flight} />

        {flight.aircraft &&
        <>
        <h5>Extra:</h5>
        <ul>
            <li><a href={`https://globe.adsbexchange.com/?icao=${flight.aircraft.icao.toLowerCase()}`} target="_blank" rel="noreferrer">Track your flight with ADS-B</a></li>
            <li><a href="https://www.aerolopa.com/" target="_blank" rel="noreferrer">Use below information to find seating plan</a></li>
            <li>Airline: {flight.airline}</li>
            <li>Model: {flight.aircraft.modelName}</li>
        </ul></>}
        
        <TrackFlightButton flightId={flightId!} userFlightId={flight.userFlightId} />
        </>
    );
}

export default Flight;


function getDisclaimer(displayGates: boolean, displayTerminals: boolean) {
    let text = `NB! This airport usually 
        ${displayTerminals ? "displays" : "doesn't display"} terminals and 
        ${displayGates ? "displays" : "doesn't display"} gates.`;
    return (
        <p className="disclaimer">{text}</p>
    );
}