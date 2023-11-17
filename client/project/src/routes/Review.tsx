import { useContext, useEffect, useState } from "react";
import { JwtContext } from "./Root";
import { IReview } from "../domain/IReview";
import { getUserId } from "../helpers/IdentityHelpers";
import { ReviewService } from "../services/ReviewService";
import { useNavigate, useParams } from "react-router-dom";
import ReviewCategories from "../components/ReviewCategories";
import ReviewStars from "../components/ReviewStars";
import { TNavigationItem } from "../dto/TNavigationItem";
import NavigationLink from "../components/NavigationLink";


const reviewService = new ReviewService();

const Review = () => {
    
    const {airportIata, reviewId} = useParams<{airportIata: string, reviewId: string | undefined}>();
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const [rating, setRating] = useState(0);
    const [selectedCategoryId, setSelectedCategoryId] = useState("");
    const [review, setReview] = useState(null as IReview | null);
    const [submitType, setSubmitType] = useState("");
    const navigate = useNavigate();
    reviewService.initJwtResponse(jwtResponse, setJwtResponse);


    useEffect(() => {
        let isCancelled = false;
        const fetchReview = async () => {
            if (!reviewId) return;

            const fetchedReview = await reviewService.get(reviewId);
            if (fetchedReview && !isCancelled) {
                if (fetchedReview.authorId === getUserId(reviewService.jwtResponse)) {
                    setReview(fetchedReview);
                    setRating(fetchedReview.rating);
                    setSelectedCategoryId(fetchedReview.category.id!);
                }
            }
        }
        fetchReview();
        return () => {
            isCancelled = true;
        }
    },[reviewId])


    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (selectedCategoryId === "" || !airportIata) return;
    
        const reviewForm = e.target as HTMLFormElement;
        const reviewInput = reviewForm["reviewValue"] as HTMLInputElement;
    
        const review: IReview = {
            id: reviewId,
            airportIata: airportIata,
            text: reviewInput.value,
            rating: rating,
            category: {
                id: selectedCategoryId,
                category: ""
            },
            authorName: "",
            authorId: getUserId(jwtResponse) ?? "",
            createdAt: new Date(),
            userFeedback: 0,
            usersFeedback: 0
        }

        const url = `/airports/${airportIata}/reviews`;
        switch (submitType) {
            case "add":
                if (review.rating < 1 || review.rating > 5 || review.text.length < 1 || review.text.length > 1000) return;
                await reviewService.add(review);
                navigate(url);
                return;
            case "save":
                if (review.rating < 1 || review.rating > 5 || review.text.length < 1 || review.text.length > 1000) return;
                await reviewService.update(review);
                navigate(url);
                return;
            case "delete":
                await reviewService.delete(reviewId ?? "");
                navigate(url);
                return;
        }
    }


   return( 
        <>
        <ReviewNavigationLinks airportIata={airportIata} />
        <div className="review-page">
        {!jwtResponse && <h2>Sign in</h2>}
        {jwtResponse && 
        <>
            <h1>{review ? "Edit" : "Add"} review</h1>
            <ReviewCategories setDefaultCategory={reviewId ? false : true} selectedCategoryId={selectedCategoryId} setSelectedCategoryId={setSelectedCategoryId} setPage={undefined}/>
            <form onSubmit={handleSubmit}>
                <br/>
                <ReviewStars rating={rating} setRating={setRating} />
                <br/>
                <textarea name="reviewValue" defaultValue={review?.text ?? ""} />
                <br/>
                {(jwtResponse && !review) && 
                    <ReviewButton method="Add" setSubmitType={setSubmitType}/>
                }
                {(jwtResponse && review) &&
                    <>
                    <ReviewButton method="Save" setSubmitType={setSubmitType}/>
                    <ReviewButton method="Delete" setSubmitType={setSubmitType}/>
                    </>
                }
            </form>
        </>}
        </div>
    </>)
}

const ReviewButton = (props: {method: string, setSubmitType: React.Dispatch<React.SetStateAction<string>>}) => {
    return (
        <button
            onClick={() => {props.setSubmitType(props.method.toLowerCase())}} 
            type="submit">{props.method}
        </button>
    );
}

const ReviewNavigationLinks = (props: {airportIata: string | undefined}) => {
    if (!props.airportIata) return <></>;
    const items: TNavigationItem[] = [
        [`/airports/`, `airports`],
        [`/airports/${props.airportIata}/`, `${props.airportIata}`],
        [`/airports/${props.airportIata}/reviews/`, `reviews`],
    ];
    return <NavigationLink items={items}  lastItem="edit"/>;
}


export default Review;