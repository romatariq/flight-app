import { IReview } from "../domain/IReview";
import { JwtContext } from "../routes/Root";
import { useContext, useState } from "react";
import { ReviewReactionService } from "../services/ReviewReactionService";


const reviewReactionService = new ReviewReactionService();

const ReviewReaction = (props: {review: IReview}) => {
    
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const [usersFeedback, setUsersFeedback] = useState(props.review.usersFeedback);
    const [userFeedback, setUserFeedback] = useState(props.review.userFeedback);
    reviewReactionService.initJwtResponse(jwtResponse, setJwtResponse);

    const handleSubmit = async (feedback: number) => {
        if (feedback !== 1 && feedback !== -1) return;
        if (userFeedback === feedback) feedback = 0;

        let reviewReaction: true | undefined;

        if (feedback === 0) {
            reviewReaction = await reviewReactionService.delete(props.review.id!);
        } else if (userFeedback === 0) {
            reviewReaction = await reviewReactionService.add(props.review.id!, feedback);
        } else if (userFeedback !== 0) {
            reviewReaction = await reviewReactionService.update(props.review.id!, feedback);
        }

        if (reviewReaction) {
            if (feedback === 0) {
                setUsersFeedback(c => c - userFeedback);
            } else {
                setUsersFeedback(c => c + (userFeedback === 0 ? 1 : 2) * feedback);
            }
            setUserFeedback(feedback);
        }
    }

    return (
        <div>
            <span>{usersFeedback}</span>            
            {jwtResponse && <button className={"user-feedback" + (userFeedback === 1 ? " selected-positive" : "")} onClick={() => {handleSubmit(1)}}>+</button>}
            {jwtResponse && <button className={"user-feedback" + (userFeedback === -1 ? " selected-negative" : "")} onClick={() => {handleSubmit(-1)}}>-</button>}
        </div>
    )
}

export default ReviewReaction;