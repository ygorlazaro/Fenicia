import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface IUser {
    id: string;
    email: string;
    name: string;
    companyId?: string;
}

interface AuthState {
    token: string | null;
    refreshToken: string | null;
    user: IUser | null;
    companyId: string | null;
    companyName: string | null;
    isAuthenticated: boolean;
}

const initialState: AuthState = {
    token: typeof window !== "undefined" ? localStorage.getItem("auth_token") : null,
    refreshToken: typeof window !== "undefined" ? localStorage.getItem("refresh_token") : null,
    user: typeof window !== "undefined" && localStorage.getItem("user") ? JSON.parse(localStorage.getItem("user")!) : null,
    companyId: typeof window !== "undefined" ? localStorage.getItem("companyId") : null,
    companyName: typeof window !== "undefined" ? localStorage.getItem("company_name") : null,
    isAuthenticated: Boolean(typeof window !== "undefined" && localStorage.getItem("auth_token"))
};

const authSlice = createSlice({
    name: "auth",
    initialState,
    reducers: {
        setCredentials(state, action: PayloadAction<{ token: string; refreshToken: string; user: IUser }>) {
            state.token = action.payload.token;
            state.refreshToken = action.payload.refreshToken;
            state.user = action.payload.user;
            state.isAuthenticated = true;
            localStorage.setItem("auth_token", action.payload.token);
            localStorage.setItem("refresh_token", action.payload.refreshToken);
            localStorage.setItem("user", JSON.stringify(action.payload.user));
        },
        setCompany(state, action: PayloadAction<{ companyId: string; companyName: string }>) {
            state.companyId = action.payload.companyId;
            state.companyName = action.payload.companyName;
            localStorage.setItem("companyId", action.payload.companyId);
            localStorage.setItem("company_name", action.payload.companyName);
        },
        logout(state) {
            state.token = null;
            state.refreshToken = null;
            state.user = null;
            state.companyId = null;
            state.companyName = null;
            state.isAuthenticated = false;
            localStorage.removeItem("auth_token");
            localStorage.removeItem("refresh_token");
            localStorage.removeItem("user");
            localStorage.removeItem("companyId");
            localStorage.removeItem("company_name");
        }
    }
});

export const { setCredentials, setCompany, logout } = authSlice.actions;

export default authSlice.reducer;
