import NavigationLink from "../components/NavigationLink";
import { IUserFlightsStatistics } from "../domain/IUserFlightsStatistics";
import { FormatMinutes } from "../helpers/TimeHelpers";
import { StatisticsService } from "../services/StatisticsService";
import { JwtContext } from "./Root";
import { useContext, useEffect, useState } from "react";

const statsService = new StatisticsService();
const UserFlightsStatistics = () => {
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const [userFlightsStatistics, setUserFlightsStatistics] = useState(null as IUserFlightsStatistics | null);
    statsService.initJwtResponse(jwtResponse, setJwtResponse);

    useEffect(() => {
        let isCancelled = false;
        const fetchData = async () => {
            const res = await statsService.getUserFlightsStatistics();
            if (res && !isCancelled) {
                setUserFlightsStatistics(res);
            }
        };

        fetchData();
        return () => {
            isCancelled = true;
        };
    }, []);


    if (!jwtResponse) {
        return <></>
    };

    return (
        <div className="text-center">
            <NavigationLink items={[["/flights", "my-flights"]]} lastItem="statistics" />
            <h1 className="text-center">My tracked flights statistics</h1>

            {userFlightsStatistics && 
            <div className="carousel-outer">
                <div id="carouselExample" className="carousel slide">
                    <div className="carousel-indicators">
                        <button type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide-to="0" className="active" aria-current="true" aria-label="Slide 1"></button>
                        <button type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide-to="1" aria-label="Slide 2"></button>
                        <button type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide-to="2" aria-label="Slide 3"></button>
                        <button type="button" data-bs-target="#carouselExampleCaptions" data-bs-slide-to="3" aria-label="Slide 4"></button>
                    </div>
                    <div className="carousel-inner">
                        <div className="carousel-item text-center p-4 active">
                             <p>Number of flights: {userFlightsStatistics.count}</p>
                        </div>
                        <div className="carousel-item text-center p-4">
                            <p>Total flight time: {FormatMinutes(userFlightsStatistics.totalTimeMinutes)}</p>
                        </div>
                        <div className="carousel-item text-center p-4">
                            <p>Total time {userFlightsStatistics.totalTimeDelayedDepartureMinutes < 0 ? "saved " : "spent "} 
                                waiting for flight to take off: {FormatMinutes(Math.abs(userFlightsStatistics.totalTimeDelayedDepartureMinutes))}</p>
                        </div>
                        <div className="carousel-item text-center p-4">
                            <p>Total time {userFlightsStatistics.totalTimeDelayedArrivalMinutes < 0 ? "saved " : "spent "} 
                                waiting for flight to land: {FormatMinutes(Math.abs(userFlightsStatistics.totalTimeDelayedArrivalMinutes))}</p>
                        </div>
                    </div>
                    <button className="carousel-control-prev" type="button" data-bs-target="#carouselExample" data-bs-slide="prev">
                        <span className="carousel-control-prev-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Previous</span>
                    </button>
                    <button className="carousel-control-next" type="button" data-bs-target="#carouselExample" data-bs-slide="next">
                        <span className="carousel-control-next-icon" aria-hidden="true"></span>
                        <span className="visually-hidden">Next</span>
                    </button>
                </div>
            </div>}
        </div>
    );
};

export default UserFlightsStatistics;