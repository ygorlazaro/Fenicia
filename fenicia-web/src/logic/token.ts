import { UserResponse } from "@/types/responses/UserResponse";

export const setToken = (token: string) => { 
    localStorage.setItem("token", token);
}

export const getToken = () => {
    return localStorage.getItem("token");
}

export const removeToken = () => {
    localStorage.removeItem("token");
}

export const setUser = (user: UserResponse) => {
    localStorage.setItem("user", JSON.stringify(user));
}

export const getUser = (): UserResponse => {
    return JSON.parse(localStorage.getItem("user") || "{}");
}

export const removeUser = () => {
    localStorage.removeItem("user");
}
