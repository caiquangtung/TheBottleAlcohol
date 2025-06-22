import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface User {
  id: number;
  fullName: string;
  email: string;
  role: string;
}

interface AuthState {
  user: User | null;
  accessToken: string | null;
  refreshToken: string | null; // Có thể null vì backend xử lý qua HttpOnly cookie
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;
}

// Load thông tin từ localStorage nếu có
const loadAuthFromStorage = (): Partial<AuthState> => {
  if (typeof window === "undefined") return {};

  const user = localStorage.getItem("user");
  const accessToken = localStorage.getItem("accessToken");
  // Không load refresh token từ localStorage - backend xử lý qua HttpOnly cookie

  if (user && accessToken) {
    return {
      user: JSON.parse(user),
      accessToken,
      refreshToken: null, // Refresh token được xử lý bởi backend qua cookie
      isAuthenticated: true,
    };
  }
  return {};
};

const initialState: AuthState = {
  user: null,
  accessToken: null,
  refreshToken: null,
  isAuthenticated: false,
  loading: false,
  error: null,
  ...loadAuthFromStorage(),
};

export const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    loginSuccess: (
      state,
      action: PayloadAction<{
        user: User;
        accessToken: string;
        refreshToken?: string | null;
      }>
    ) => {
      state.loading = false;
      state.isAuthenticated = true;
      state.user = action.payload.user;
      state.accessToken = action.payload.accessToken;
      state.refreshToken = action.payload.refreshToken || null;
      state.error = null;
    },
    logout: (state) => {
      // Xóa thông tin khỏi localStorage (refresh token được xử lý bởi backend)
      localStorage.removeItem("user");
      localStorage.removeItem("accessToken");
      // Không xóa refresh token - backend sẽ xử lý qua cookie

      state.user = null;
      state.accessToken = null;
      state.refreshToken = null;
      state.isAuthenticated = false;
      state.error = null;
    },
    setLoading: (state, action: PayloadAction<boolean>) => {
      state.loading = action.payload;
    },
    setError: (state, action: PayloadAction<string | null>) => {
      state.error = action.payload;
    },
    clearError: (state) => {
      state.error = null;
    },
  },
});

export const { loginSuccess, logout, setLoading, setError, clearError } =
  authSlice.actions;

export default authSlice.reducer;
