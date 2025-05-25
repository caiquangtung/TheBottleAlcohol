"use client";
import { Provider } from "react-redux";
import { store } from "./index";
import { useEffect } from "react";
import { useAppDispatch } from "./hooks";
import { loginSuccess } from "../features/auth/authSlice";

function AuthLoader() {
  const dispatch = useAppDispatch();
  useEffect(() => {
    const user = localStorage.getItem("user");
    const accessToken = localStorage.getItem("accessToken");
    const refreshToken = localStorage.getItem("refreshToken");
    if (user && accessToken && refreshToken) {
      dispatch(
        loginSuccess({
          user: JSON.parse(user),
          accessToken,
          refreshToken,
        })
      );
    }
  }, [dispatch]);
  return null;
}

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <Provider store={store}>
      <AuthLoader />
      {children}
    </Provider>
  );
}
