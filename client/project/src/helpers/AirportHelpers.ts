import { IFlight } from "../domain/IFlight";

export function getCorrectAirportName(flight: IFlight, getDeparture: boolean, getMinimalName: boolean = false) {
    const name = getDeparture ? flight.departureAirportName : flight.arrivalAirportName;
    if (getMinimalName) {
        return minifyAirportName(name);
    }
    return name;
}

function minifyAirportName(name: string) {
    const airportIndex = name.toLowerCase().indexOf(" airport");
    name = airportIndex > 0 ? name.substring(0, airportIndex) : name;
    
    const internationalIndex = name.toLowerCase().indexOf(" international");
    name = internationalIndex > 0 ? name.substring(0, internationalIndex) : name;
    
    const regionalIndex = name.toLowerCase().indexOf(" regional");
    name = regionalIndex > 0 ? name.substring(0, regionalIndex) : name;
    return name;
}

export function getCorrectAirportIata(flight: IFlight, getDeparture: boolean) {
    return getDeparture ? flight.departureAirportIata : flight.arrivalAirportIata;
}

export function getAirportText(flight: IFlight, getDeparture: boolean) {
    return getCorrectAirportName(flight, getDeparture, true) + " " + getCorrectAirportIata(flight, getDeparture);
}

export function getAirportTitle(airportName: string, airportIata: string) {
    return minifyAirportName(airportName) + " Airport (" + airportIata + ")";
}

export function getAirportTitleMinified(airportName: string, airportIata: string) {
    return minifyAirportName(airportName) + " " + airportIata;
}