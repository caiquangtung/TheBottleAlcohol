"use client";
import Image from "next/image";
import { Input } from "../ui/input";
import { Button } from "../ui/button";
import { Heart, ShoppingCart, Sun, Moon, User } from "lucide-react";
import { useSelector } from "react-redux";
import { RootState } from "../../lib/store";
import { useAppDispatch, useAppSelector } from "../../lib/store/hooks";
import { toggleDark } from "../../lib/features/theme/themeSlice";
import { toggleCartDrawer } from "../../lib/features/cart/cartSlice";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { CartDrawer } from "./CartDrawer";
import { Badge } from "../ui/badge";
import { useState, useEffect } from "react";
import {
  useGetWishlistsByCustomerQuery,
  useGetWishlistProductsQuery,
} from "@/lib/services/wishlistService";

export default function Header() {
  const [isHydrated, setIsHydrated] = useState(false);
  const isDark = useSelector((state: RootState) => state.theme.isDark);
  const dispatch = useAppDispatch();
  const router = useRouter();
  const user = useAppSelector((state) => state.auth.user);
  const userId = user?.id;
  const cartDetails = useAppSelector((state) => state.cart.cartDetails);

  // Lấy wishlist đầu tiên của user
  const { data: wishlists } = useGetWishlistsByCustomerQuery(
    typeof userId === "number" ? userId : -1,
    { skip: typeof userId !== "number" }
  );
  const wishlistId = wishlists?.[0]?.id;
  const { data: wishlistProducts } = useGetWishlistProductsQuery(
    typeof wishlistId === "number" ? wishlistId : -1,
    { skip: typeof wishlistId !== "number" }
  );
  const wishlistCount = wishlistProducts?.length || 0;

  // Hydration check
  useEffect(() => {
    setIsHydrated(true);
  }, []);

  const totalCartItems =
    cartDetails?.reduce(
      (total: number, item: { quantity?: number }) =>
        total + (item?.quantity || 0),
      0
    ) || 0;

  const handleUserClick = () => {
    if (user) {
      router.push("/profile");
    } else {
      router.push("/login");
    }
  };

  const handleWishlistClick = () => {
    router.push("/wishlist");
  };

  // Don't render until hydrated
  if (!isHydrated) {
    return (
      <header className="w-full border-b bg-background/80 dark:bg-[#18181b]/80 sticky top-0 z-10 transition-colors duration-300">
        <div className="container mx-auto flex flex-col gap-2 py-4 px-2">
          <div className="flex items-center justify-between gap-4 w-full">
            <Link href="/">
              <div className="flex items-center gap-4">
                <Image
                  src="/Logo.png"
                  alt="Logo"
                  width={200}
                  height={200}
                  className="transition-all dark:invert"
                />
              </div>
            </Link>
            <div className="flex-1 flex justify-center">
              <Input placeholder="Search..." className="w-full max-w-xl" />
            </div>
            <div className="flex items-center gap-2 min-w-[180px] justify-end">
              <Button variant="ghost" size="icon">
                <User className="h-5 w-5" />
              </Button>
              <Button variant="ghost" size="icon" onClick={handleWishlistClick}>
                <Heart className="h-5 w-5" />
                {wishlistCount > 0 && (
                  <Badge
                    variant="secondary"
                    className="absolute -top-2 -right-2 h-5 w-5 flex items-center justify-center p-1"
                  >
                    {wishlistCount}
                  </Badge>
                )}
              </Button>
              <Button variant="ghost" size="icon" className="relative">
                <ShoppingCart className="h-5 w-5" />
              </Button>
              <Button variant="ghost" size="icon">
                <Moon className="h-5 w-5" />
              </Button>
            </div>
          </div>
        </div>
      </header>
    );
  }

  return (
    <header className="w-full border-b bg-background/80 dark:bg-[#18181b]/80 sticky top-0 z-10 transition-colors duration-300">
      <div className="container mx-auto flex flex-col gap-2 py-4 px-2">
        <div className="flex items-center justify-between gap-4 w-full">
          {/* Logo bên trái */}
          <Link href="/">
            <div className="flex items-center gap-4">
              <Image
                src="/Logo.png"
                alt="Logo"
                width={200}
                height={200}
                className="transition-all dark:invert"
              />
            </div>
          </Link>
          {/* Search bar ở giữa */}
          <div className="flex-1 flex justify-center">
            <Input placeholder="Search..." className="w-full max-w-xl" />
          </div>
          {/* Icon bên phải */}
          <div className="flex items-center gap-2 min-w-[180px] justify-end">
            <Button variant="ghost" size="icon" onClick={handleUserClick}>
              <User className="h-5 w-5" />
            </Button>
            <Button variant="ghost" size="icon" onClick={handleWishlistClick}>
              <Heart className="h-5 w-5" />
              {wishlistCount > 0 && (
                <Badge
                  variant="secondary"
                  className="absolute -top-2 -right-2 h-5 w-5 flex items-center justify-center p-1"
                >
                  {wishlistCount}
                </Badge>
              )}
            </Button>
            <Button
              variant="ghost"
              size="icon"
              className="relative"
              onClick={() => dispatch(toggleCartDrawer())}
            >
              {totalCartItems > 0 && (
                <Badge
                  variant="destructive"
                  className="absolute -top-2 -right-2 h-5 w-5 flex items-center justify-center p-1"
                >
                  {totalCartItems}
                </Badge>
              )}
              <ShoppingCart className="h-5 w-5" />
            </Button>
            {/* Dark mode toggle button with icon */}
            <Button
              variant="ghost"
              size="icon"
              aria-label="Toggle dark mode"
              onClick={() => dispatch(toggleDark())}
            >
              {isDark ? (
                <Sun className="h-5 w-5" />
              ) : (
                <Moon className="h-5 w-5" />
              )}
            </Button>
          </div>
        </div>
        <nav className="w-full flex justify-center"></nav>
      </div>
      <CartDrawer />
    </header>
  );
}
