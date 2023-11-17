import { IReview } from "../domain/IReview";
import { IReviewCategory } from "../domain/IReviewCategory";
import { IReviews } from "../dto/IReviews";
import { BaseAuthorizedService } from "./BaseAuthorizedService";

export class ReviewService extends BaseAuthorizedService {
    constructor(){
        super('v1/reviews/');
    }

    async getCategories(): Promise<IReviewCategory[] | undefined> {
        try {
            const response = await this.axios.get('categories');
            
            if (response.status === 200) {
                return response.data;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async getAll(airportIata: string, categoryId: string, page: number): Promise<IReviews | undefined> {
        try {
            const jwt = await this.GetValidJwt();
            const response = await this.axios.get(
                jwt ? 'getAllAuthorized' : "getAll", 
                {
                    params: {
                        'airportIata': airportIata,
                        'page': page,
                        'categoryId': categoryId
                    },
                    headers: {
                        'Authorization': 'Bearer ' + jwt?.jwt
                    }
                }
            );
            
            if (response.status === 200) {
                const res = response.data as IReviews;
                res.data.forEach(r => r.createdAt = new Date(r.createdAt!));
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async get(reviewId: string): Promise<IReview | undefined> {
        try {
            const jwt = await this.GetValidJwt();
            const response = await this.axios.get(
                'get', 
                {               
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                    params: {
                        'reviewId': reviewId
                    }
                }
            );
            
            if (response.status === 200) {
                const res = response.data as IReview;
                res.createdAt = new Date(res.createdAt!);
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async add(review: IReview): Promise<IReview | undefined> {
        try {
            const jwt = await this.GetValidJwt();
            const response = await this.axios.post(
                'add',
                review,
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    }
                }
            );
            
            if (response.status === 200) {
                const res = response.data as IReview;
                res.createdAt = new Date(res.createdAt!);
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async delete(reviewId: string): Promise<true | undefined> {
        try {
            const jwt = await this.GetValidJwt();
            const response = await this.axios.delete(
                'delete', 
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    },
                    params: {
                        "id": reviewId
                    }
                }
            );
            
            if (response.status === 200) {
                return true;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }

    async update(review: IReview): Promise<IReview | undefined> {
        try {
            const jwt = await this.GetValidJwt();
            const response = await this.axios.put(
                'update',
                review,
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
                    }
                }
            );
            
            if (response.status === 200) {
                const res = response.data as IReview;
                res.createdAt = new Date(res.createdAt!);
                return res;
            }
            return undefined;
            
        } catch (e) {
            return undefined;
        }
    }
}
