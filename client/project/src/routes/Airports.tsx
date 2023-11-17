import { useEffect, useState } from "react";
import { AirportService } from "../services/AirportService";
import { IAirport } from "../domain/IAirport";
import AirportCard from "../components/AirportCard";

const airportService = new AirportService();

const Airports = () => {
    const [filter, setFilter] = useState("");
    const [airports, setAirports] = useState([] as IAirport[]);
    const [fetching, setFetching] = useState(false);

    const handleSumbit = (event: React.FormEvent) => {
        event.preventDefault();
        
        const input = event.target as HTMLFormElement;
        const filterValue: string = input["filterValue"].value;

        setFilter(filterValue);
    }

    useEffect(() => {
        let isCancelled = false;
        const fetchCountries = async () => {
            setFetching(true);
            const airportsList = await airportService.getAirports(filter);
            if (airportsList && !isCancelled) {
                setAirports(airportsList);
                setFetching(false);
            } 
        };
        fetchCountries();
        return () => {
            isCancelled = true;
        }
    }, [filter])


    return <>
        <div>
            <form onSubmit={handleSumbit} >
            <input
                name="filterValue" type="text" placeholder="Search airport by name, iata or country"
                className="search-bar" />
            </form>
        </div>
        <div className="d-flex flex-wrap" >
        {
            airports
            .map((airport) => {
                return <AirportCard airport={airport} key={airport.id} />
            })
        }
        </div>
        {(airports.length === 0 && !fetching) && <h1>No airports found!</h1>}
    </>

}



export default Airports;
