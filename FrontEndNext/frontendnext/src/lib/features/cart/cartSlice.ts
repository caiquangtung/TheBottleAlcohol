import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Cart, CartDetail } from "@/lib/types/cart";
import { Product } from "@/lib/types/product";

interface CartState {
  items: CartDetail[];
  rowVersion: string | null;
  isCartDrawerOpen: boolean;
}

const initialState: CartState = {
  items: [],
  rowVersion: null,
  isCartDrawerOpen: false,
};

const cartSlice = createSlice({
  name: "cart",
  initialState,
  reducers: {
    toggleCartDrawer: (state) => {
      state.isCartDrawerOpen = !state.isCartDrawerOpen;
    },
    setCartData: (state, action: PayloadAction<Cart>) => {
      state.items = action.payload.items || [];
      state.rowVersion = action.payload.rowVersion;
    },
    addItem: (
      state,
      action: PayloadAction<{ product: Product; quantity: number }>
    ) => {
      if (!state.items) {
        state.items = [];
      }

      const { product, quantity } = action.payload;
      const existingItem = state.items.find(
        (item) => item.productId === product.id
      );
      if (existingItem) {
        existingItem.quantity += quantity;
      } else {
        state.items.push({
          productId: product.id,
          product: product,
          quantity: quantity,
          price: product.price,
        });
      }
      state.isCartDrawerOpen = true;
    },
    removeItem: (state, action: PayloadAction<number>) => {
      if (!state.items) {
        state.items = [];
        return;
      }

      state.items = state.items.filter(
        (item) => item.productId !== action.payload
      );
    },
    updateItemQuantity: (
      state,
      action: PayloadAction<{ productId: number; quantity: number }>
    ) => {
      if (!state.items) {
        state.items = [];
        return;
      }

      const { productId, quantity } = action.payload;
      const itemToUpdate = state.items.find(
        (item) => item.productId === productId
      );
      if (itemToUpdate) {
        if (quantity > 0) {
          itemToUpdate.quantity = quantity;
        } else {
          state.items = state.items.filter(
            (item) => item.productId !== productId
          );
        }
      }
    },
    clearCart: (state) => {
      state.items = [];
      state.rowVersion = null;
    },
    rehydrateCart: (state) => {
      if (!state.items) {
        state.items = [];
      }
      if (state.rowVersion === undefined) {
        state.rowVersion = null;
      }
      if (state.isCartDrawerOpen === undefined) {
        state.isCartDrawerOpen = false;
      }
    },
  },
  extraReducers: (builder) => {
    builder.addMatcher(
      (action) => action.type.endsWith("/rehydrate"),
      (state) => {
        if (!state.items) {
          state.items = [];
        }
        if (state.rowVersion === undefined) {
          state.rowVersion = null;
        }
        if (state.isCartDrawerOpen === undefined) {
          state.isCartDrawerOpen = false;
        }
      }
    );
  },
});

export const {
  toggleCartDrawer,
  setCartData,
  addItem,
  removeItem,
  updateItemQuantity,
  clearCart,
  rehydrateCart,
} = cartSlice.actions;
export default cartSlice.reducer;
