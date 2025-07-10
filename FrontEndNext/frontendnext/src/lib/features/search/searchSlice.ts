import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface SearchState {
  isSearchOverlayOpen: boolean;
  searchTerm: string;
  isSearching: boolean;
}

const initialState: SearchState = {
  isSearchOverlayOpen: false,
  searchTerm: "",
  isSearching: false,
};

const searchSlice = createSlice({
  name: "search",
  initialState,
  reducers: {
    toggleSearchOverlay: (state) => {
      state.isSearchOverlayOpen = !state.isSearchOverlayOpen;
      if (!state.isSearchOverlayOpen) {
        // Reset search when closing overlay
        state.searchTerm = "";
        state.isSearching = false;
      }
    },
    openSearchOverlay: (state) => {
      state.isSearchOverlayOpen = true;
    },
    closeSearchOverlay: (state) => {
      state.isSearchOverlayOpen = false;
      state.searchTerm = "";
      state.isSearching = false;
    },
    setSearchTerm: (state, action: PayloadAction<string>) => {
      state.searchTerm = action.payload;
    },
    setIsSearching: (state, action: PayloadAction<boolean>) => {
      state.isSearching = action.payload;
    },
  },
});

export const {
  toggleSearchOverlay,
  openSearchOverlay,
  closeSearchOverlay,
  setSearchTerm,
  setIsSearching,
} = searchSlice.actions;

export default searchSlice.reducer;
