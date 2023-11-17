import { useEffect, useRef } from "react";
import { getAirportText } from "../helpers/AirportHelpers";
import { IFlightDetails } from "../domain/IFlightDetails";
import { Link } from "react-router-dom";

const FlightHeader = (props: {flight: IFlightDetails | undefined}) => {
    const h1Ref = useRef<HTMLSpanElement>(null);
    const progressBarRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        if (!h1Ref.current || !progressBarRef.current) {
            return;
        }
        progressBarRef.current.style.width = h1Ref.current.offsetWidth + "px";
    }, [props.flight])


    if (!props.flight) {
        return <></>;
    }

    return(
        <>
        <h1 className="text-center">
            <span ref={h1Ref} >
                <Link to={`/airports/${props.flight.departureAirportIata}`} className="black-link silent-link">{getAirportText(props.flight, true)}</Link>
                {" -> "}
                <Link to={`/airports/${props.flight.arrivalAirportIata}`} className="black-link silent-link">{getAirportText(props.flight, false)}</Link>
            </span>
        </h1>
        <div className="progress-bar-outer">
            <div className="progress-bar" ref={progressBarRef} >
                <div className="purple" style={{"width": props.flight.percentageOfFlightDone + "%"}}></div>
            </div>
        </div>
        </>
    )
}

export default FlightHeader;