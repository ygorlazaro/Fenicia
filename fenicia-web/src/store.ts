import { legacy_createStore as createStore } from 'redux'
import { TypedUseSelectorHook, useSelector } from 'react-redux'

interface AppState {
  sidebarShow: boolean
  theme: string
  sidebarUnfoldable?: boolean
}

interface Action {
  type: string
  [key: string]: any
}

const initialState: AppState = {
  sidebarShow: true,
  theme: 'light',
}

const changeState = (state: AppState = initialState, { type, ...rest }: Action): AppState => {
  switch (type) {
    case 'set':
      return { ...state, ...rest } as AppState
    default:
      return state
  }
}

const store = createStore(changeState)

// Export types
export type RootState = ReturnType<typeof changeState>
export type AppDispatch = typeof store.dispatch

// Create typed hooks for usage instead of plain useDispatch/useSelector
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector

export default store
