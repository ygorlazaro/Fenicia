import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface UiState {
    sidebarShow: boolean;
    sidebarUnfoldable: boolean;
}

const initialState: UiState = {
    sidebarShow: true,
    sidebarUnfoldable: false
};

const uiSlice = createSlice({
    name: "ui",
    initialState,
    reducers: {
        setSidebarShow(state, action: PayloadAction<boolean>) {
            state.sidebarShow = action.payload;
        },
        setSidebarUnfoldable(state, action: PayloadAction<boolean>) {
            state.sidebarUnfoldable = action.payload;
        }
    }
});

export const { setSidebarShow, setSidebarUnfoldable } = uiSlice.actions;

export default uiSlice.reducer;
