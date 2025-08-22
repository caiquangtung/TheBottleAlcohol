import { configureStore } from "@reduxjs/toolkit";
import { api } from "../services/api";
import themeReducer from "../features/theme/themeSlice";
import authReducer from "../features/auth/authSlice";
import cartReducer from "../features/cart/cartSlice";
import searchReducer from "../features/search/searchSlice";
import localeReducer from "../features/locale/localeSlice";

export const store = configureStore({
  reducer: {
    [api.reducerPath]: api.reducer,
    theme: themeReducer,
    auth: authReducer,
    cart: cartReducer,
    search: searchReducer,
    locale: localeReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(api.middleware),
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;
