import { useEffect, useState } from "react";
import { StatisticsService } from "../services/StatisticsService";
import { IAirportStatistics } from "../domain/IAirportStatistics";
import { useParams } from "react-router-dom";
import { getAirportTitle } from "../helpers/AirportHelpers";
import "../styles/statistics.css";
import StatistricsBlock from "../components/StatisticsBlock";
import Loading from "../components/Loading";
import { TNavigationItem } from "../dto/TNavigationItem";
import NavigationLink from "../components/NavigationLink";

const statisticsService = new StatisticsService();
const AirportStatistics = () => {
    const [airportStatistics, setAirportStatistics] = useState(null as IAirportStatistics | null);
    const { airportIata } = useParams<{airportIata: string}>() ;
    const [selectedTimePeriodHours, setselectedTimePeriodHours] = useState(1);
    const [isFetching, setIsFetching] = useState(true);


    useEffect(() => {
        let isCancelled = false;
        const fetchStats = async () => {
            if (!airportIata) return;
            setIsFetching(true);
            const fetchedStats = await statisticsService.getAirportStatistics(airportIata, selectedTimePeriodHours);
            if (fetchedStats && !isCancelled) {
                setAirportStatistics(fetchedStats);
            }
            if (fetchedStats) {
                setIsFetching(false);
            }
        }
        fetchStats();
        return () => {
            isCancelled = true;
        }
    }, [airportIata, selectedTimePeriodHours])

    if (!airportStatistics && isFetching) {
        return <Loading></Loading>
    }

    if (!airportStatistics || !airportIata) {
        return <></>
    }


    return (
        <>
            <NavigationLinks airportIata={airportIata}/>
            <div className="text-center">
                <h1>{getAirportTitle(airportStatistics.airportName, airportStatistics.airportIata)}</h1>
                <TimeButtons selectedTimePeriodHours={selectedTimePeriodHours} setSelectedTimePeriodHours={setselectedTimePeriodHours} isFetching={isFetching} />
                {airportStatistics.departuresCount !== 0 && 
                    <>
                        <h2 className="text-left stats-category">Departures ({airportStatistics.departuresCount} flights)</h2>
                        <StatistricsBlock list={airportStatistics.departureAirlines} totalCount={airportStatistics.departuresCount} title="Airlines"/>
                        <StatistricsBlock list={airportStatistics.departureCountries} totalCount={airportStatistics.departuresCount} title="Countries"/>
                    </>
                }
                <hr></hr>
                {airportStatistics.arrivalsCount !== 0 && 
                    <>
                        <h2 className="text-left stats-category">Arrivals ({airportStatistics.arrivalsCount} flights)</h2>
                        <StatistricsBlock list={airportStatistics.arrivalAirlines} totalCount={airportStatistics.arrivalsCount} title="Airlines"/>
                        <StatistricsBlock list={airportStatistics.arrivalCountries} totalCount={airportStatistics.arrivalsCount} title="Countries"/>
                    </>
                }
            </div>
        </>
    )
}

const NavigationLinks = (props: {airportIata: string}) => {
    const items: TNavigationItem[] = [
        ["/airports/", "airports"],
        [`/airports/${props.airportIata}`, `${props.airportIata}`]
    ];
    return <NavigationLink items={items} lastItem="statistics" />
}

const TimeButtons = (props: {selectedTimePeriodHours: number, setSelectedTimePeriodHours: React.Dispatch<React.SetStateAction<number>>, isFetching: boolean}) => {
    const timePeriods = [1, 24, 7*24, 30*7*24, 0];
    const timePeriodNames = ["Hourly", "Daily", "Weekly", "Monthly", "All time"];

    return (
        <>
            {timePeriods.map((n, i) => {
                return (
                    <button key={n} 
                        className={(props.selectedTimePeriodHours === n ? "btn btn-primary" : "btn btn-outline-primary")}
                        disabled={props.isFetching}
                        onClick={() => props.setSelectedTimePeriodHours(n)}>
                    {timePeriodNames[i]}</button>
                )
            })}
        </>
    )
}


export default AirportStatistics;