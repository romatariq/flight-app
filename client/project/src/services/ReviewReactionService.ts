import { BaseAuthorizedService } from "./BaseAuthorizedService";

export class ReviewReactionService extends BaseAuthorizedService {
    constructor(){
        super('v1/reviewReaction/');
    }
    

    async add(reviewId: string, feedback: number): Promise<true | undefined> {

        const a: any = {
            reviewId,
            feedback
        }
        try {        
            const jwt = await this.GetValidJwt();    
            const response = await this.axios.post(
                'add',
                a,
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
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
                        reviewId,
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

    async update(reviewId: string, feedback: number): Promise<true | undefined> {

        const a: any = {
            reviewId,
            feedback
        }
        try {        
            const jwt = await this.GetValidJwt();    
            const response = await this.axios.put(
                'update',
                a,
                {
                    headers: {
                        'Authorization': 'Bearer ' + jwt!.jwt
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



}
