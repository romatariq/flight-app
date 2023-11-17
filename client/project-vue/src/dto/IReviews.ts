import { type IAirport } from "../domain/IAirport";
import { type IReview } from "../domain/IReview";
import { type IBaseResponseWithPaging } from "./base/IBaseResponseWithPaging";

export interface IReviews extends IBaseResponseWithPaging<IReview> {
    airport: IAirport
}
