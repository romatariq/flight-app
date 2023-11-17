import { IAirport } from "../domain/IAirport";
import { IReview } from "../domain/IReview";
import { IBaseResponseWithPaging } from "./base/IBaseResponseWithPaging";

export interface IReviews extends IBaseResponseWithPaging<IReview> {
    airport: IAirport
}
