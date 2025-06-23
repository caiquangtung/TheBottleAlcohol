"use client";
import { Provider } from "react-redux";
import { store } from "./index";
import { useEffect } from "react";
import { useAppDispatch } from "./hooks";
import { loginSuccess } from "../features/auth/authSlice";
import { rehydrateCart } from "../features/cart/cartSlice";

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
      {children}
    </Provider>
  );
}
