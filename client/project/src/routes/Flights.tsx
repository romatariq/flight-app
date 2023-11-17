import { useContext, useEffect, useState } from "react";
import { FlightService } from "../services/FlightService";
import { JwtContext } from "./Root";
import Error from "../components/Error";
import { IFlight } from "../domain/IFlight";
import AirportFlights from "../components/AirportFlights";
import { useParams } from "react-router-dom";


const flightService = new FlightService();

const Flights = (props: {isDeparture: boolean} ) => {
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const [flights, setFlights] = useState([] as IFlight[]);
    const [errors, setErrors] = useState([] as string[]);
    const [fetching, setFetching] = useState(false);
    const { airportIata } = useParams<{airportIata: string}>();
    flightService.initJwtResponse(jwtResponse, setJwtResponse);


    useEffect(() => {
        let isCancelled = false;
        const fetchFlights = async () => {
            
            if (!airportIata || airportIata.length !== 3) {
                setErrors(["Invalid url!"]);
                return;
            }

            setFetching(true);
            const fetchedFlights = props.isDeparture ? 
                await flightService.getDepartures(airportIata) :
                await flightService.getArrivals(airportIata);
                
            if (fetchedFlights && !isCancelled) {
                setFlights(fetchedFlights);
                setFetching(false);

            } else if (!isCancelled) {
                setErrors(["Flights not displayed for this airport!"]);
                setFetching(false);
            }
        };
        fetchFlights();
        return () => {
            isCancelled = true;
        }
    }, [airportIata, props.isDeparture]);
    

    if (errors.length > 0) {
      return <Error errorMessage={errors[0]}/>;
    }
    
 
    return (
          <AirportFlights isDeparture={props.isDeparture} flights={flights} fetching={fetching} airportIata={airportIata} />
      );

}

export default Flights;
