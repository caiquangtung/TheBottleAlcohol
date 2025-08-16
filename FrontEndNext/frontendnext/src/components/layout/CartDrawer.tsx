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
import { Cart, CartDetail, CartSyncPayload } from "@/lib/types/cart";
import { useRouter } from "next/navigation";

export function CartDrawer() {
  const router = useRouter();
  const dispatch = useAppDispatch();
  const isCartDrawerOpen = useAppSelector(
    (state: { cart: { isCartDrawerOpen: boolean } }) =>
      state.cart.isCartDrawerOpen
  );
  const cartDetails = useAppSelector(
    (state: { cart: { cartDetails: CartDetail[] } }) => state.cart.cartDetails
  );
  const rowVersion = useAppSelector(
    (state: { cart: { rowVersion: string | null } }) => state.cart.rowVersion
  );
  const user = useAppSelector((state) => state.auth.user);
  const userId = user?.id;

  const {
    data: cartData,
    refetch: refetchCart,
  }: {
    data?: Cart;
    refetch: () => Promise<{ data?: Cart }>;
  } = useGetCartQuery(typeof userId === "number" ? userId : -1, {
    skip: !isCartDrawerOpen || typeof userId !== "number",
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
          customerId: userId || 0,
          cartDetails: [],
          totalAmount: 0,
          rowVersion: null,
          createdAt: "",
          updatedAt: null,
          customerName: null,
        })
      );
    }
  }, [isCartDrawerOpen, cartData, dispatch, userId]);

  const totalPrice = useMemo(() => {
    return (cartDetails as CartDetail[]).reduce(
      (total: number, item: CartDetail) =>
        total + (item?.price || 0) * (item?.quantity || 0),
      0
    );
  }, [cartDetails]);

  const handleToggle = (isOpen: boolean) => {
    if (!isOpen) {
      dispatch(toggleCartDrawer());
    }
  };

  const handleSyncCart = async () => {
    if (typeof userId !== "number") {
      toast.error("Please log in to sync cart.");
      return;
    }

    try {
      const syncPayload: CartSyncPayload = {
        customerId: userId,
        items: (cartDetails as CartDetail[]).map((item: CartDetail) => ({
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

  const handleCheckout = async () => {
    if (typeof userId !== "number") {
      toast.error("Please log in to continue.");
      return;
    }
    if (!cartDetails || (cartDetails as CartDetail[]).length === 0) {
      toast.error("Your cart is empty.");
      return;
    }
    try {
      // Optionally sync cart then navigate to checkout page
      try {
        await handleSyncCart();
      } catch {}
      dispatch(toggleCartDrawer());
      router.push("/checkout");
    } catch {
      toast.error("Failed to proceed to checkout.");
    }
  };

  return (
    <Sheet open={isCartDrawerOpen} onOpenChange={handleToggle}>
      <SheetContent className="flex w-full flex-col pr-0 sm:max-w-lg z-[110]">
        <SheetHeader className="px-6">
          <SheetTitle>Cart ({(cartDetails || []).length})</SheetTitle>
        </SheetHeader>
        <div className="flex-1 overflow-y-auto">
          <div className="flex flex-col gap-6 px-6 py-4">
            {(cartDetails as CartDetail[]).length === 0 ? (
              <div className="flex flex-col items-center justify-center h-full text-center">
                <p className="text-lg font-semibold">Your cart is empty</p>
                <p className="text-muted-foreground">
                  Add some products to get started!
                </p>
              </div>
            ) : (
              (cartDetails as CartDetail[]).map((item: CartDetail) => (
                <div
                  key={item.productId}
                  className="flex items-center gap-4 p-4 border rounded-lg"
                >
                  <div className="w-16 h-16 relative">
                    <Image
                      src={item.productImageUrl || "/placeholder.jpg"}
                      alt={item.productName}
                      fill
                      className="object-cover rounded"
                    />
                  </div>
                  <div className="flex-1">
                    <h4 className="font-medium">{item.productName}</h4>
                    <p className="text-sm text-muted-foreground">
                      ${(item.price || 0).toFixed(2)}
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
        {(cartDetails as CartDetail[]).length > 0 && (
          <SheetFooter className="flex-col gap-4 px-6 py-4 border-t">
            <div className="flex justify-between items-center">
              <span className="text-lg font-semibold">Total:</span>
              <span className="text-lg font-bold">
                ${totalPrice.toFixed(2)}
              </span>
            </div>
            <div className="flex gap-2">
              <Button
                variant="outline"
                onClick={handleSyncCart}
                className="flex-1"
              >
                Sync Cart
              </Button>
              <Button className="flex-1" onClick={handleCheckout}>
                Checkout
              </Button>
            </div>
          </SheetFooter>
        )}
      </SheetContent>
    </Sheet>
  );
}
