import { TokenRequest } from '@/types/requests/TokenRequest'
import { TokenResponse } from '@/types/responses/TokenResponse'
import axios from 'axios'

export class TokenService { 
    private readonly axios = axios.create({
        baseURL: "http://localhost:5144/token",
    })

    public async getToken(tokenRequest: TokenRequest): Promise<TokenResponse> {
        const response = await this.axios.post("/", tokenRequest)
     
        return response.data;
    }
}


