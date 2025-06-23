import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { Cart, CartDetail } from "@/lib/types/cart";
import { Product } from "@/lib/types/product";

interface CartState {
  cartDetails: CartDetail[];
  rowVersion: string | null;
  isCartDrawerOpen: boolean;
}

const initialState: CartState = {
  cartDetails: [],
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
      state.cartDetails = action.payload.cartDetails || [];
      state.rowVersion = action.payload.rowVersion;
    },
    addItem: (
      state,
      action: PayloadAction<{ product: Product; quantity: number }>
    ) => {
      if (!state.cartDetails) {
        state.cartDetails = [];
      }

      const { product, quantity } = action.payload;
      const existingItem = state.cartDetails.find(
        (item) => item.productId === product.id
      );
      if (existingItem) {
        existingItem.quantity += quantity;
      } else {
        state.cartDetails.push({
          id: 0,
          cartId: 0,
          productId: product.id,
          productName: product.name,
          productImageUrl: product.imageUrl || "",
          price: product.price,
          quantity: quantity,
          createdAt: new Date().toISOString(),
          updatedAt: null,
        });
      }
      state.isCartDrawerOpen = true;
    },
    removeItem: (state, action: PayloadAction<number>) => {
      if (!state.cartDetails) {
        state.cartDetails = [];
        return;
      }

      state.cartDetails = state.cartDetails.filter(
        (item) => item.productId !== action.payload
      );
    },
    updateItemQuantity: (
      state,
      action: PayloadAction<{ productId: number; quantity: number }>
    ) => {
      if (!state.cartDetails) {
        state.cartDetails = [];
        return;
      }

      const { productId, quantity } = action.payload;
      const itemToUpdate = state.cartDetails.find(
        (item) => item.productId === productId
      );
      if (itemToUpdate) {
        if (quantity > 0) {
          itemToUpdate.quantity = quantity;
        } else {
          state.cartDetails = state.cartDetails.filter(
            (item) => item.productId !== productId
          );
        }
      }
    },
    clearCart: (state) => {
      state.cartDetails = [];
      state.rowVersion = null;
    },
    rehydrateCart: (state) => {
      if (!state.cartDetails) {
        state.cartDetails = [];
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
        if (!state.cartDetails) {
          state.cartDetails = [];
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
