import { ApiResponse } from "@/types/ApiResponse";
import { ModuleResponse } from "@/types/responses/ModuleResponse";
import axios from "axios";

export class UserService {
    private readonly axios = axios.create({
        baseURL: "http://localhost:5144/user",
    })

    private get token() {
        return localStorage.getItem("token");
    }

    public async modules(): Promise<ApiResponse<ModuleResponse[]>> {
        const response = await this.axios.get("/module", {
            headers: { 
                Authorization: "Bearer " + this.token
            }
        })

        return response.data;
    }
}
