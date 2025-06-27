"use client";
import { useAppSelector } from "@/lib/store/hooks";
import {
  useGetWishlistsByCustomerQuery,
  useGetWishlistProductsQuery,
} from "@/lib/services/wishlistService";
import { useMemo } from "react";
import ProductCard from "@/components/ProductCard";
import { useGetProductsByIdsQuery } from "@/lib/services/productService";
import { WishlistDetail } from "@/lib/types/wishlist";
import { Product } from "@/lib/types/product";

export default function WishlistPage() {
  const user = useAppSelector((state) => state.auth.user);
  const userId = user?.id;
  const { data: wishlists } = useGetWishlistsByCustomerQuery(
    typeof userId === "number" ? userId : -1,
    { skip: typeof userId !== "number" }
  );
  const wishlistId = wishlists?.[0]?.id;
  const { data: wishlistProducts = [] } = useGetWishlistProductsQuery(
    typeof wishlistId === "number" ? wishlistId : -1,
    { skip: typeof wishlistId !== "number" }
  );

  // Lấy danh sách productId từ wishlist
  const productIds = useMemo(
    () =>
      (wishlistProducts as WishlistDetail[]).map(
        (item: WishlistDetail) => item.productId
      ),
    [wishlistProducts]
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
          {(products as Product[]).map((product: Product) => (
            <ProductCard key={product.id} product={product} />
          ))}
        </div>
      )}
    </div>
  );
}
