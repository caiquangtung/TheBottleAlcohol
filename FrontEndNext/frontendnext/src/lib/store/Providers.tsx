"use client";
import { Provider } from "react-redux";
import { store } from "./index";
import { useEffect } from "react";
import { useAppDispatch } from "./hooks";
import { loginSuccess } from "../features/auth/authSlice";
import { rehydrateCart } from "../features/cart/cartSlice";
import { useSelector } from "react-redux";
import { RootState } from ".";
import { setDark } from "../features/theme/themeSlice";

function AuthLoader() {
  const dispatch = useAppDispatch();
  useEffect(() => {
    const user = localStorage.getItem("user");
    // Không lấy accessToken từ localStorage nữa
    if (user) {
      dispatch(
        loginSuccess({
          user: JSON.parse(user),
          accessToken: null, // accessToken sẽ được refresh qua cookie nếu cần
        })
      );
    }
  }, [dispatch]);
  return null;
}

function CartLoader() {
  const dispatch = useAppDispatch();
  useEffect(() => {
    // Ensure cart state is properly initialized
    dispatch(rehydrateCart());
  }, [dispatch]);
  return null;
}

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <Provider store={store}>
      <AuthLoader />
      <CartLoader />
      <ThemeInitializer />
      <ThemeApplier />
      {children}
    </Provider>
  );
}

function ThemeInitializer() {
  const dispatch = useAppDispatch();
  useEffect(() => {
    try {
      const saved = localStorage.getItem("isDark");
      if (saved !== null) {
        dispatch(setDark(saved === "true"));
      } else if (
        window.matchMedia &&
        window.matchMedia("(prefers-color-scheme: dark)").matches
      ) {
        dispatch(setDark(true));
      }
    } catch {}
  }, [dispatch]);
  return null;
}

function ThemeApplier() {
  const isDark = useSelector((s: RootState) => s.theme.isDark);
  useEffect(() => {
    try {
      const root = document.documentElement;
      if (isDark) {
        root.classList.add("dark");
      } else {
        root.classList.remove("dark");
      }
      localStorage.setItem("isDark", String(isDark));
    } catch {}
  }, [isDark]);
  return null;
}
