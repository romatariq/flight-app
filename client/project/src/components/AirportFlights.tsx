import React, { useState } from "react";
import { IFlight } from "../domain/IFlight";
import { getNowUtc, getTimezoneOffsetMinutes, getTime } from "../helpers/TimeHelpers";
import { getAirportText, getCorrectAirportName, getCorrectAirportIata } from "../helpers/AirportHelpers";
import { Link } from "react-router-dom";
import "../styles/flights.css";
import Loading from "./Loading";
import AirportClock from "./AirportClock";

const AirportFlights = (props: { flights: IFlight[], isDeparture: boolean, fetching: boolean, airportIata: string | undefined }) => {

    const [showPreviousFlights, setShowPreviousFlights] = useState(false);
    let timeOffSetInMinutes: number | undefined;

    if (props.flights.length > 0) {
        const utcDate = props.isDeparture ? props.flights[0].scheduledDepartureUtc : props.flights[0].scheduledArrivalUtc;
        const localDate = props.isDeparture ? props.flights[0].scheduledDepartureLocal : props.flights[0].scheduledArrivalLocal;
        timeOffSetInMinutes = getTimezoneOffsetMinutes(utcDate, localDate);
    }

    if (props.fetching) {
        return <Loading/>
    }

    return (
        <>
        <div className="back-link">
            <Link to={"/airports/"}>airports</Link> / <Link to={`/airports/${props.airportIata}`}>{props.airportIata}</Link> / {props.isDeparture ? "departures" : "arrivals"}
        </div>
        <h1 className="text-center">{props.flights.length === 0 ? "" : 
            (getCorrectAirportName(props.flights[0], props.isDeparture) + " " + getCorrectAirportIata(props.flights[0], props.isDeparture))
            }
        </h1>
        <div className="flights-box">
            <div className="flight-box-header">
                <h1 className="header">{props.isDeparture ? "Departures": "Arrivals"}</h1>
                <AirportClock timeOffSetInMinutes={timeOffSetInMinutes}/>
            </div>
            <div className="flights-row header">
                <div style={{"width" : "10%"}}>TIME</div>
                <div style={{"width" : "15%"}}>AIRLINE</div>
                <div style={{"width" : "10%"}}></div>
                <div style={{"width" : "15%"}}>FLIGHT</div>
                <div style={{"width" : "30%"}}>{props.isDeparture ? "TO" : "FROM" }</div>
                <div style={{"width" : "20%"}}>STATUS</div>
            </div>
            <div style={{"textAlign" : "center"}}>
                <div className="button"
                    onClick={() => setShowPreviousFlights(!showPreviousFlights)}>
                    {showPreviousFlights ? "HIDE" : "SHOW"} PREVIOUS FLIGHTS
                </div>
            </div>
                  
            
            {props.flights
            .sort((a: IFlight, b: IFlight) => {
                const dateA = props.isDeparture ? a.scheduledDepartureLocal : a.scheduledArrivalLocal;
                const dateB = props.isDeparture ? b.scheduledDepartureLocal : b.scheduledArrivalLocal;
                return dateA.getTime() - dateB.getTime();
            })
            .filter((flight) => {
                if (showPreviousFlights) return true;
                return new Date(props.isDeparture ? flight.scheduledDepartureUtc : flight.scheduledArrivalUtc).getTime() > getNowUtc().getTime();
            })
            .map((flight) => {
                return <AirportFlight flight={flight} isDeparture={props.isDeparture} key={flight.id}/>
            })}
        </div>
        </>
    );
}

const AirportFlight = (props: { flight: IFlight, isDeparture: boolean }) => {
    return (
        <React.Fragment key={props.flight.id}>
        <div className="flights-row data">
            <div style={{"width" : "10%"}}>{getTime(props.isDeparture ? props.flight.scheduledDepartureLocal : props.flight.scheduledArrivalLocal)}</div>
            <div style={{"width" : "15%"}}>{props.flight.airline}</div>
            <div style={{"width" : "10%"}}></div>
            <div style={{"width" : "15%"}}><Link to={`flight/${props.flight.id}`}>{props.flight.flightIata}</Link></div>
            <div style={{"width" : "30%"}}>{getAirportText(props.flight, !props.isDeparture)}</div>
            <div style={{"width" : "20%"}}>{props.flight.status}</div>
        </div>
        <hr/>
        </React.Fragment>
    );
}

export default AirportFlights;
