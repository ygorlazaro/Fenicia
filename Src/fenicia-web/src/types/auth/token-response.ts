import { UserResponse } from "./user-response";

export type TokenResponse = {
    accessToken: string;
    refreshToken: string;
    user: UserResponse;
};
