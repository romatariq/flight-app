import { IFlightDetails } from "../domain/IFlightDetails";
import { FormatMinutes } from "../helpers/TimeHelpers";

const FlightComparisonTable = (props: {flight: IFlightDetails}) => {
    return (
        <div className="flight-comparison-table">
            <h3 className="text-center">Trip ({Math.round(props.flight.flightDistanceKm)}km) rough comparison to other transports</h3>
            <table>
                <tr>
                    <th>Vehicle</th>
                    <th>kg of CO2 per person</th>
                    <th>Average time</th>
                    <th>Specifications</th>
                </tr>
                <tr>
                    <td>Plane</td>
                    <td>{Math.round(props.flight.planeKgOfCo2PerPerson)}</td>
                    <td>{FormatMinutes(props.flight.planeTravelTimeMinutes)}</td>
                    <td>190 people, actual travel time {FormatMinutes((props.flight.estimatedArrivalUtc.getTime() - props.flight.estimatedDepartureUtc.getTime()) / 1000 / 60)}</td>
                </tr>
                <tr>
                    <td>Car (petrol)</td>
                    <td>{Math.round(props.flight.carKgOfCo2PerPerson)}</td>
                    <td>{FormatMinutes(props.flight.carTravelTimeMinutes)}</td>
                    <td>5 people</td>
                </tr>
                <tr>
                    <td>Electric train</td>
                    <td>{Math.round(props.flight.trainKgOfCo2PerPerson)}</td>
                    <td>{FormatMinutes(props.flight.trainTravelTimeMinutes)}</td>
                    <td>200 people</td>
                </tr>
                <tr>
                    <td>Cruise ship</td>
                    <td>{Math.round(props.flight.shipKgOfCo2PerPerson)}</td>
                    <td>{FormatMinutes(props.flight.shipTravelTimeMinutes)}</td>
                    <td>2000 people</td>
                </tr>
            </table>
        </div>
    );
};

export default FlightComparisonTable;