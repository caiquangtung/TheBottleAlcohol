"use client";
import { useAppSelector } from "@/lib/store/hooks";
import {
  useGetWishlistsByCustomerQuery,
  useGetWishlistProductsQuery,
  useRemoveProductFromWishlistMutation,
} from "@/lib/services/wishlistService";
import { useMemo, useState } from "react";
import { Product } from "@/lib/types/product";
import { WishlistDetail } from "@/lib/types/wishlist";
import { toast } from "sonner";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Heart, Trash2 } from "lucide-react";
import ProductCard from "@/components/ProductCard";
import {
  useGetProductByIdQuery,
  useGetProductsByIdsQuery,
} from "@/lib/services/productService";

export default function WishlistPage() {
  const user = useAppSelector((state) => state.auth.user);
  const userId = user?.id;
  const { data: wishlists } = useGetWishlistsByCustomerQuery(
    typeof userId === "number" ? userId : -1,
    { skip: typeof userId !== "number" }
  );
  const wishlistId = wishlists?.[0]?.id;
  const { data: wishlistProducts = [], refetch } = useGetWishlistProductsQuery(
    typeof wishlistId === "number" ? wishlistId : -1,
    { skip: typeof wishlistId !== "number" }
  );
  const [removeProductFromWishlist] = useRemoveProductFromWishlistMutation();
  const [optimisticProducts, setOptimisticProducts] = useState<
    WishlistDetail[] | null
  >(null);

  const handleRemove = async (productId: number) => {
    if (!wishlistId) return;
    const prevProducts = wishlistProducts.slice();
    setOptimisticProducts(
      (optimisticProducts || wishlistProducts).filter(
        (item) => item.productId !== productId
      )
    );
    try {
      await removeProductFromWishlist({ wishlistId, productId }).unwrap();
      toast.success("Removed from wishlist");
      refetch();
    } catch {
      toast.error("Failed to remove from wishlist");
      setOptimisticProducts(prevProducts);
    }
  };

  // Lấy danh sách productId từ wishlist
  const productIds = useMemo(
    () =>
      (optimisticProducts ?? wishlistProducts).map((item) => item.productId),
    [optimisticProducts, wishlistProducts]
  );
  // Gọi 1 API lấy toàn bộ sản phẩm wishlist
  const { data: products = [], isLoading: productsLoading } =
    useGetProductsByIdsQuery(productIds, { skip: productIds.length === 0 });

  return (
    <div className="container mx-auto py-10">
      <h1 className="text-3xl font-bold mb-6">My Wishlist</h1>
      {productsLoading ? (
        <div>Loading products...</div>
      ) : products.length === 0 ? (
        <div className="text-muted-foreground">Your wishlist is empty.</div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {products.map((product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      )}
    </div>
  );
}
