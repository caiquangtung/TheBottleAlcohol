import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export interface ThemeState {
  isDark: boolean;
}

const initialState: ThemeState = {
  isDark: false,
};

const themeSlice = createSlice({
  name: "theme",
  initialState,
  reducers: {
    setDark: (state, action: PayloadAction<boolean>) => {
      state.isDark = action.payload;
    },
    toggleDark: (state) => {
      state.isDark = !state.isDark;
    },
  },
});

export const { setDark, toggleDark } = themeSlice.actions;
export default themeSlice.reducer;
