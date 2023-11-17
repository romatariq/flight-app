import { TNavigationItem } from "../dto/TNavigationItem";
import NavigationLink from "./NavigationLink";

const FlightNavigationLink = (props: { airportIata: string | undefined, isDeparture: boolean, flightIata: string }) => {

    if (!props.airportIata) {
        const items: TNavigationItem[] = [["/flights/", "flights"]];
        return <NavigationLink items={items} lastItem={props.flightIata}  />
    };

    const items: TNavigationItem[] = [
        ["/airports/", "airports"],
        [`/airports/${props.airportIata}/`, `${props.airportIata}`],
        [`/airports/${props.airportIata}/${props.isDeparture ? "departures" : "arrivals"}`, `${props.isDeparture ? "departures" : "arrivals"}`],
    ]
    return NavigationLink({ items: items, lastItem: props.flightIata });
}

export default FlightNavigationLink;