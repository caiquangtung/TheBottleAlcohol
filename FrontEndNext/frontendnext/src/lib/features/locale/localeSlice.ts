import { createSlice, PayloadAction } from "@reduxjs/toolkit";

export type SupportedLocale = "vi" | "en";

interface LocaleState {
  current: SupportedLocale;
}

function getInitialLocale(): SupportedLocale {
  if (typeof window === "undefined") return "vi";
  try {
    const stored = localStorage.getItem("locale") as SupportedLocale | null;
    if (stored === "vi" || stored === "en") return stored;
  } catch {}
  return "vi";
}

const initialState: LocaleState = {
  current: getInitialLocale(),
};

const localeSlice = createSlice({
  name: "locale",
  initialState,
  reducers: {
    setLocale(state, action: PayloadAction<SupportedLocale>) {
      state.current = action.payload;
      try {
        if (typeof window !== "undefined") {
          localStorage.setItem("locale", action.payload);
        }
      } catch {}
    },
    toggleLocale(state) {
      state.current = state.current === "vi" ? "en" : "vi";
      try {
        if (typeof window !== "undefined") {
          localStorage.setItem("locale", state.current);
        }
      } catch {}
    },
  },
});

export const { setLocale, toggleLocale } = localeSlice.actions;
export default localeSlice.reducer;
