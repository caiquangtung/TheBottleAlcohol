"use client";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetFooter,
  SheetClose,
} from "@/components/ui/sheet";
import { Button } from "../ui/button";
import { useAppSelector, useAppDispatch } from "@/lib/store/hooks";
import {
  removeItem,
  updateItemQuantity,
  toggleCartDrawer,
  setCartData,
} from "@/lib/features/cart/cartSlice";
import Image from "next/image";
import { Minus, Plus, Trash2 } from "lucide-react";
import {
  useSyncCartMutation,
  useGetCartQuery,
} from "@/lib/services/cartService";
import { useMemo, useEffect } from "react";
import { toast } from "sonner";
import { Cart, CartSyncPayload } from "@/lib/types/cart";

export function CartDrawer() {
  const dispatch = useAppDispatch();
  const {
    isCartDrawerOpen,
    items = [],
    rowVersion,
  } = useAppSelector(
    (state) =>
      state.cart || { isCartDrawerOpen: false, items: [], rowVersion: null }
  );
  const {
    data: cartData,
    refetch: refetchCart,
  }: {
    data?: Cart;
    refetch: () => Promise<{ data?: Cart }>;
  } = useGetCartQuery(undefined, {
    skip: !isCartDrawerOpen,
  });

  const [syncCart] = useSyncCartMutation();

  // Auto-refresh cart when drawer opens
  useEffect(() => {
    if (isCartDrawerOpen && cartData) {
      dispatch(setCartData(cartData));
    } else if (isCartDrawerOpen && !cartData) {
      dispatch(
        setCartData({
          id: 0,
          userId: "",
          items: [],
          totalPrice: 0,
          rowVersion: null,
        })
      );
    }
  }, [isCartDrawerOpen, cartData, dispatch]);

  const totalPrice = useMemo(() => {
    return (items || []).reduce(
      (total, item) => total + (item?.price || 0) * (item?.quantity || 0),
      0
    );
  }, [items]);

  const handleToggle = (isOpen: boolean) => {
    if (!isOpen) {
      dispatch(toggleCartDrawer());
    }
  };

  const handleSyncCart = async () => {
    try {
      const syncPayload: CartSyncPayload = {
        items: (items || []).map((item) => ({
          productId: item.productId,
          quantity: item.quantity,
        })),
        rowVersion: rowVersion,
      };

      const data: Cart = await syncCart(syncPayload).unwrap();
      dispatch(setCartData(data));
      toast.success("Cart synchronized successfully!");
    } catch (error: unknown) {
      if (
        error &&
        typeof error === "object" &&
        "status" in error &&
        error.status === 409
      ) {
        toast.error("Cart has changed on the server. Refreshing...");
        try {
          const { data: refreshedCart }: { data?: Cart } = await refetchCart();
          if (refreshedCart) {
            dispatch(setCartData(refreshedCart));
            toast.success("Cart refreshed successfully!");
          }
        } catch {
          toast.error("Failed to refresh cart. Please reload the page.");
        }
      } else {
        const errorMessage =
          error &&
          typeof error === "object" &&
          "data" in error &&
          error.data &&
          typeof error.data === "object" &&
          "message" in error.data
            ? String(error.data.message)
            : "Failed to sync cart.";
        toast.error(errorMessage);
      }
    }
  };

  return (
    <Sheet open={isCartDrawerOpen} onOpenChange={handleToggle}>
      <SheetContent className="flex w-full flex-col pr-0 sm:max-w-lg">
        <SheetHeader className="px-6">
          <SheetTitle>Cart ({(items || []).length})</SheetTitle>
        </SheetHeader>
        <div className="flex-1 overflow-y-auto">
          <div className="flex flex-col gap-6 px-6 py-4">
            {(items || []).length === 0 ? (
              <div className="flex flex-col items-center justify-center h-full text-center">
                <p className="text-lg font-semibold">Your cart is empty</p>
                <p className="text-muted-foreground">
                  Add some products to get started!
                </p>
              </div>
            ) : (
              (items || []).map((item) => (
                <div key={item.productId} className="flex items-center gap-4">
                  <div className="relative h-20 w-20 flex-shrink-0 overflow-hidden rounded-md">
                    <Image
                      src={item.product?.imageUrl || "/placeholder.png"}
                      alt={item.product?.name || "Product"}
                      fill
                      className="object-cover"
                    />
                  </div>
                  <div className="flex-1">
                    <p className="font-medium">
                      {item.product?.name || "Unknown Product"}
                    </p>
                    <p className="text-sm text-muted-foreground">
                      {item.quantity || 0} x ${(item.price || 0).toFixed(2)}
                    </p>
                  </div>
                  <div className="flex items-center gap-2">
                    <Button
                      variant="outline"
                      size="icon"
                      className="h-8 w-8"
                      onClick={() =>
                        dispatch(
                          updateItemQuantity({
                            productId: item.productId,
                            quantity: (item.quantity || 0) - 1,
                          })
                        )
                      }
                    >
                      <Minus className="h-4 w-4" />
                    </Button>
                    <span className="w-8 text-center">
                      {item.quantity || 0}
                    </span>
                    <Button
                      variant="outline"
                      size="icon"
                      className="h-8 w-8"
                      onClick={() =>
                        dispatch(
                          updateItemQuantity({
                            productId: item.productId,
                            quantity: (item.quantity || 0) + 1,
                          })
                        )
                      }
                    >
                      <Plus className="h-4 w-4" />
                    </Button>
                  </div>
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => dispatch(removeItem(item.productId))}
                  >
                    <Trash2 className="h-5 w-5 text-destructive" />
                  </Button>
                </div>
              ))
            )}
          </div>
        </div>
        {(items || []).length > 0 && (
          <SheetFooter className="gap-2 bg-background p-6">
            <div className="flex w-full items-center justify-between">
              <p className="text-lg font-semibold">Total:</p>
              <p className="text-lg font-semibold">${totalPrice.toFixed(2)}</p>
            </div>
            <Button className="w-full" onClick={handleSyncCart}>
              Sync Cart
            </Button>
            <SheetClose asChild>
              <Button className="w-full">Continue to Checkout</Button>
            </SheetClose>
          </SheetFooter>
        )}
      </SheetContent>
    </Sheet>
  );
}
