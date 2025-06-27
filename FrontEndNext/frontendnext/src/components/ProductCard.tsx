import React from "react";
import Link from "next/link";
import Image from "next/image";
import { Product } from "@/lib/types/product";
import { Button } from "./ui/button";
import { Card } from "./ui/card";
import { Badge } from "./ui/badge";
import { Heart } from "lucide-react";
import { useAppSelector, useAppDispatch } from "@/lib/store/hooks";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { addItem } from "@/lib/features/cart/cartSlice";
import { useSyncCartMutation } from "@/lib/services/cartService";
import {
  useGetWishlistsByCustomerQuery,
  useGetWishlistProductsQuery,
  useAddProductToWishlistMutation,
  useRemoveProductFromWishlistMutation,
} from "@/lib/services/wishlistService";
import { useMemo, useState } from "react";
import { WishlistDetail } from "@/lib/types/wishlist";
import { useCreateWishlistMutation } from "@/lib/services/wishlistService";

interface ProductCardProps {
  product: Product;
  className?: string;
}

const formatPrice = (price: number) => {
  return new Intl.NumberFormat("en-GB", {
    style: "currency",
    currency: "GBP",
  }).format(price);
};

const formatVolume = (volume: number) => {
  return `${volume.toFixed(2)}CL`;
};

const formatABV = (abv: number) => {
  return `${abv.toFixed(1)}%`;
};

const ProductCard: React.FC<ProductCardProps> = ({ product, className }) => {
  const dispatch = useAppDispatch();
  const router = useRouter();
  const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
  const cartDetails = useAppSelector((state) => state.cart.cartDetails);
  const rowVersion = useAppSelector((state) => state.cart.rowVersion);
  const [syncCart, { isLoading }] = useSyncCartMutation();
  const user = useAppSelector((state) => state.auth.user);
  const userId = user?.id;
  const { data: wishlists } = useGetWishlistsByCustomerQuery(
    typeof userId === "number" ? userId : -1,
    { skip: typeof userId !== "number" }
  );
  const wishlistId = wishlists?.[0]?.id;
  const { data: wishlistProducts, refetch } = useGetWishlistProductsQuery(
    typeof wishlistId === "number" ? wishlistId : -1,
    { skip: typeof wishlistId !== "number" }
  );
  const [addProductToWishlist] = useAddProductToWishlistMutation();
  const [removeProductFromWishlist] = useRemoveProductFromWishlistMutation();
  const [createWishlist] = useCreateWishlistMutation();
  const isInWishlist = useMemo(
    () =>
      wishlistProducts?.some(
        (item: WishlistDetail) => item.productId === product.id
      ),
    [wishlistProducts, product.id]
  );
  const [loading, setLoading] = useState(false);

  // Check if product is already in cart
  const isInCart = cartDetails.some(
    (item: import("@/lib/types/cart").CartDetail) =>
      item.productId === product.id
  );
  const cartQuantity =
    cartDetails.find(
      (item: import("@/lib/types/cart").CartDetail) =>
        item.productId === product.id
    )?.quantity || 0;

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.preventDefault(); // Prevent navigation to product page
    e.stopPropagation();

    if (!isAuthenticated) {
      toast.error("Please log in to add items to your cart.");
      router.push("/login");
      return;
    }

    if (product.stockQuantity === 0) {
      toast.error("This product is out of stock.");
      return;
    }

    try {
      dispatch(addItem({ product, quantity: 1 }));

      await syncCart({
        items: [
          ...cartDetails.filter(
            (item: import("@/lib/types/cart").CartDetail) =>
              item.productId !== product.id
          ),
          {
            productId: product.id,
            quantity:
              (cartDetails.find(
                (item: import("@/lib/types/cart").CartDetail) =>
                  item.productId === product.id
              )?.quantity || 0) + 1,
          },
        ].map((item: { productId: number; quantity: number }) => ({
          productId: item.productId,
          quantity: item.quantity,
        })),
        rowVersion,
      }).unwrap();

      toast.success("Item added to cart!");
    } catch (error: unknown) {
      console.error("Error adding to cart:", error);

      // Handle specific error cases
      if ((error as { status?: number })?.status === 409) {
        toast.error("Cart has changed. Please refresh and try again.");
      } else if ((error as { status?: number })?.status === 401) {
        toast.error("Please log in again.");
        router.push("/login");
      } else {
        toast.error("Failed to add item to cart. Please try again.");
      }
    }
  };

  const handleWishlistClick = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (!userId) {
      toast.error("Please log in to use wishlist.");
      router.push("/login");
      return;
    }
    setLoading(true);
    try {
      let currentWishlistId = wishlistId;
      // Nếu chưa có wishlist, tạo mới
      if (!currentWishlistId) {
        const newWishlist = await createWishlist({
          accountId: userId,
          name: "My Wishlist",
        }).unwrap();
        currentWishlistId = newWishlist.id;
        // refetch lại danh sách wishlist
        refetch();
      }
      if (isInWishlist) {
        await removeProductFromWishlist({
          wishlistId: currentWishlistId,
          productId: product.id,
        }).unwrap();
        toast.success("Removed from wishlist");
      } else {
        await addProductToWishlist({
          wishlistId: currentWishlistId,
          productId: product.id,
        }).unwrap();
        toast.success("Added to wishlist");
      }
      refetch();
    } catch {
      toast.error("Failed to update wishlist");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Card
      className={`relative flex flex-col overflow-hidden dark:bg-[#23232b] ${className}`}
    >
      <div className="pt-4 px-4 flex flex-col h-full">
        {product.stockQuantity < 10 && (
          <Badge className="absolute top-2 left-2 dark:bg-[#3a2a3a] dark:text-[#f96d8d] !px-3 !py-1 text-xs font-semibold tracking-wide z-10">
            LIMITED STOCK
          </Badge>
        )}
        <Button
          variant={isInWishlist ? "secondary" : "ghost"}
          size="icon"
          className="absolute top-4 right-4 z-10"
          onClick={handleWishlistClick}
          disabled={loading}
        >
          <Heart
            className={`h-5 w-5 ${
              isInWishlist ? "text-pink-500 fill-pink-500" : ""
            }`}
          />
        </Button>
        <Link
          href={`/product/${product.slug}-${product.id}`}
          className="flex flex-col flex-grow"
        >
          <div className="relative w-full aspect-[3/4] flex-shrink-0">
            <Image
              src={product.imageUrl || "/product.png"}
              alt={product.name}
              fill
              sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
              className="object-contain group-hover:scale-105 transition-transform duration-300"
            />
          </div>

          <div className="mt-2 flex flex-col flex-grow space-y-1">
            <div className="flex items-center justify-between">
              <span className="text-xs text-muted-foreground uppercase tracking-wide">
                {product.brandName}
              </span>
              <span className="text-xs text-muted-foreground uppercase">
                {product.categoryName}
              </span>
            </div>
            <h3 className="font-bold text-base line-clamp-2">{product.name}</h3>
            <div className="flex items-center justify-between w-full">
              <span className="font-bold text-lg">
                {formatPrice(product.price)}
              </span>
              <div className="flex items-center gap-1 text-sm">
                {[...Array(5)].map((_, idx) => (
                  <span key={idx} className="text-gray-300">
                    ★
                  </span>
                ))}
              </div>
            </div>
            <div className="flex items-center gap-1 text-xs text-muted-foreground">
              <span>{formatVolume(product.volume)}</span>
              <span>•</span>
              <span>{formatABV(product.alcoholContent)}</span>
              <span>•</span>
              <span>{product.origin}</span>
            </div>
          </div>
        </Link>
        <Button
          size="lg"
          className={`w-full mt-4 border-2 font-bold tracking-wide rounded-none rounded-b-lg ${
            isInCart
              ? "bg-green-600 text-white border-green-600 hover:bg-green-700"
              : "border-black bg-transparent text-black dark:bg-[#f96d8d] dark:text-black dark:border-white hover:bg-black hover:text-white dark:hover:bg-white dark:hover:text-black"
          }`}
          onClick={handleAddToCart}
          disabled={product.stockQuantity === 0 || isLoading}
        >
          {isLoading
            ? "ADDING..."
            : product.stockQuantity === 0
            ? "OUT OF STOCK"
            : isInCart
            ? `IN CART (${cartQuantity})`
            : "ADD TO CART"}
        </Button>
      </div>
    </Card>
  );
};

export const ProductCardList = ({
  products,
  className = "",
}: {
  products: Product[];
  className?: string;
}) => (
  <div
    className={`grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 ${className}`}
  >
    {products.map((product) => (
      <ProductCard key={product.id} product={product} />
    ))}
  </div>
);

export default ProductCard;
