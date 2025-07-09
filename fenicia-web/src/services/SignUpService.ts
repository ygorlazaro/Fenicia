import { SignUpRequest } from "@/types/requests/SignUpRequest";
import { SignUpResponse } from "@/types/responses/SignUpResponse";
import axios from "axios";

export class SignUpService {
    private readonly axios = axios.create({
        baseURL: "http://localhost:5144/signup",
    })

    public async signUp(signUpRequest: SignUpRequest): Promise<SignUpResponse> {
        const response = await this.axios.post("/", signUpRequest)

        return response.data;
    }
}
