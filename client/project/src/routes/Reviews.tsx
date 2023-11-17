import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Error from "../components/Error";
import "../styles/reviews.css"
import { IAirport } from "../domain/IAirport";
import { getAirportTitle } from "../helpers/AirportHelpers";
import ReviewsList from "../components/ReviewsList";
import ReviewCategories from "../components/ReviewCategories";
import { AirportService } from "../services/AirportService";
import { TNavigationItem } from "../dto/TNavigationItem";
import NavigationLink from "../components/NavigationLink";

const airportService = new AirportService();

const Reviews = () => {
    const { airportIata } = useParams<{airportIata: string}>();
    const [errors, setErrors] = useState([] as string[]);
    const [airport, setAirport] = useState(undefined as IAirport | undefined);
    const [selectedCategoryId, setSelectedCategoryId] = useState("");
    const [page, setPage] = useState(1);


    useEffect(() => {
        let isCancelled = false;
        const fetchAirport = async () => {
            if (!airportIata || airportIata.length !== 3) {
                setErrors(["Invalid url!"]);
                return;
            }

            const fetchedAirport = await airportService.getAirport(airportIata);
            if (fetchedAirport && !isCancelled) {
                setAirport(fetchedAirport);
            }
        };
        fetchAirport();
        return () => {
            isCancelled = true;
        }
    }, [airportIata])

    if (errors.length > 0) {
        return <Error errorMessage={errors[0]}/>;
    }

    return (<>
        <NavigationLinks airportIata={airportIata} />
        <div className="review-page">
            <h1>{airport && getAirportTitle(airport.name, airport.iata)}</h1>
            <ReviewCategories setDefaultCategory={true} selectedCategoryId={selectedCategoryId} setSelectedCategoryId={setSelectedCategoryId} setPage={setPage} />
            <ReviewsList airportIata={airportIata} selectedCategoryId={selectedCategoryId} setErrors={setErrors} page={page} setPage={setPage} />
        </div>
        </>
    );
}

const NavigationLinks = (props: {airportIata: string | undefined}) => {
    const items: TNavigationItem[] = [
        [`/airports/`, `airports`],
        [`/airports/${props.airportIata}/`, `${props.airportIata}`],
    ];
    return <NavigationLink items={items} lastItem="reviews" />
}

export default Reviews;
