import { IBaseEntity } from "./IBaseEntity";
import { IReviewCategory } from "./IReviewCategory";

export interface IReview extends IBaseEntity {

    category: IReviewCategory
    airportIata: string;
    authorName: string;
    authorId: string;
    text: string;
    createdAt: Date;
    rating: number;
    userFeedback: number;
    usersFeedback: number;
}
