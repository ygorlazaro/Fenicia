import { UserResponse } from "./UserResponse";

export interface TokenResponse { 
    accessToken: string;
    refreshToken: string;
    user: UserResponse;
}


