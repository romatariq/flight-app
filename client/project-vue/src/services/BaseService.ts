import Axios, { type AxiosInstance } from 'axios';

export abstract class BaseService {
    private static hostBaseURL = `${import.meta.env.VITE_BACKEND_URL}/api/`;

    protected axios: AxiosInstance;

    constructor(baseUrl: string) {

        this.axios = Axios.create(
            {
                baseURL: BaseService.hostBaseURL + baseUrl,
                headers: {
                    common: {
                        'Content-Type': 'application/json'
                        }
                }
            }
        );

        this.axios.interceptors.request.use(request => {
            // console.log('Starting Request', JSON.stringify(request, null, 2));
            console.log('Starting Request', request.url);
            return request;
        });

        this.axios.interceptors.response.use(response => {
            // console.log('Response:', JSON.stringify(response, null, 2));
            console.log('Response status: ', response.status);
            return response;
        });

    }


}
