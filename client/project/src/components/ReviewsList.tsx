import { Link } from "react-router-dom";
import { IReview } from "../domain/IReview";
import { getUserId } from "../helpers/IdentityHelpers";
import { convertUtcToLocal, getFullDateTime } from "../helpers/TimeHelpers";
import { JwtContext } from "../routes/Root";
import { useContext, useEffect, useState } from "react";
import { ReviewService } from "../services/ReviewService";
import ReviewReaction from "./ReviewReaction";
import { IJWTResponse } from "../dto/IJWTResponse";
import ReviewStars from "./ReviewStars";


const reviewService = new ReviewService();

const ReviewsList = (props: {
    airportIata: string | undefined,
    selectedCategoryId: string,
    setErrors: (errors: string[]) => void,
    page: number,
    setPage: React.Dispatch<React.SetStateAction<number>>
}) => {
    
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const [reviews, setReviews] = useState([] as IReview[]);
    const [totalPages, setTotalPages] = useState(1);
    reviewService.initJwtResponse(jwtResponse, setJwtResponse);
    

    useEffect(() => {
        let isCancelled = false;
        const fetchReviews = async () => {
            if (props.selectedCategoryId === "") return;

            if (!props.airportIata || props.airportIata.length !== 3) {
                props.setErrors(["Inavlid url!"]);
                return;
            }

            const fetchedReviews = await reviewService.getAll(props.airportIata, props.selectedCategoryId, props.page);
            if (fetchedReviews && !isCancelled) {
                if (props.page === 1) {
                    setReviews(fetchedReviews.data);
                    setTotalPages(fetchedReviews.totalPageCount);
                } else {
                    setReviews(fetchedReviews.data);
                }
            } else if (!isCancelled) {
                props.setErrors(["Failed to load reviews!"]);
            }
        };
        fetchReviews();
        return () => {
            isCancelled = true;
        }
    }, [props])

    return (
        <>
        {jwtResponse && <Link to="edit">Add review</Link>}
        {props.page > 1 && 
            <button className="btn btn-primary" onClick={() => props.setPage((p) => p - 1)}>Previous page</button>}
        {reviews
            .sort((a, b) => b.createdAt.getTime() - a.createdAt.getTime())
            .map((review) => {
                return <ReviewBox review={review} jwtResponse={jwtResponse} key={review.id} />
        })}
        {props.page < totalPages && 
            <button className="btn btn-primary" onClick={() => props.setPage((p) => p + 1)}>Next page</button>}
        </>
    );
}

const ReviewBox = (props: { review: IReview, jwtResponse: IJWTResponse | null }) => {
    return (
        <div className="review-box">
            <ReviewStars rating={props.review.rating} setRating={undefined} />
            <p>{props.review.text}</p>
            <div className="text-right">{props.review.authorName} at {getFullDateTime(convertUtcToLocal(props.review.createdAt))}</div>
            <ReviewReaction  review={props.review} />

            {props.jwtResponse && getUserId(props.jwtResponse) === props.review.authorId &&
                <div className="text-right"><Link to={`edit/${props.review.id}`}>Edit</Link></div>
            }

        </div>
    );
}

export default ReviewsList;
