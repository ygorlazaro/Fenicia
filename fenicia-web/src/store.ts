import { configureStore } from "@reduxjs/toolkit";
import { TypedUseSelectorHook, useSelector } from "react-redux";
import uiSlice from "./features/ui/uiSlice";
import authSlice from "./features/auth/authSlice";

export const store = configureStore({
    reducer: {
        ui: uiSlice,
        auth: authSlice
    },
    devTools: import.meta.env.DEV
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;

export default store;
