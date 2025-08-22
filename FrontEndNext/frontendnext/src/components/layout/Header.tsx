"use client";
import Image from "next/image";
import { Input } from "../ui/input";
import { Button } from "../ui/button";
import {
  Heart,
  ShoppingCart,
  Sun,
  Moon,
  User,
  Search,
  X,
  Languages,
} from "lucide-react";
import { useSelector } from "react-redux";
import { RootState } from "../../lib/store";
import { useAppDispatch, useAppSelector } from "../../lib/store/hooks";
import { toggleDark } from "../../lib/features/theme/themeSlice";
import { toggleCartDrawer } from "../../lib/features/cart/cartSlice";
import {
  openSearchOverlay,
  setSearchTerm,
  clearSearch,
} from "../../lib/features/search/searchSlice";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { CartDrawer } from "./CartDrawer";
import SearchOverlay from "../SearchOverlay";
import { Badge } from "../ui/badge";
import { useState, useEffect } from "react";
import { useGetMyWishlistProductsQuery } from "@/lib/services/wishlistService";
import { toggleLocale } from "@/lib/features/locale/localeSlice";
import { useI18n } from "@/lib/i18n/useI18n";

export default function Header() {
  const { t } = useI18n();
  const [isHydrated, setIsHydrated] = useState(false);
  const isDark = useSelector((state: RootState) => state.theme.isDark);
  const dispatch = useAppDispatch();
  const router = useRouter();
  const user = useAppSelector((state) => state.auth.user);
  const { searchTerm } = useAppSelector((state) => state.search);
  const userId = user?.id;
  const currentLocale = useAppSelector((s) => s.locale.current);
  const cartDetails = useAppSelector((state) => state.cart.cartDetails);

  // Lấy wishlist của user hiện tại
  const { data: wishlistProducts = [] } = useGetMyWishlistProductsQuery(
    undefined,
    { skip: !userId }
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

  const handleSearchFocus = () => {
    dispatch(openSearchOverlay());
  };

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = e.target.value;
    dispatch(setSearchTerm(value));
  };

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchTerm.trim()) {
      dispatch(openSearchOverlay());
    }
  };

  const handleClearSearch = () => {
    dispatch(clearSearch());
  };

  // Don't render until hydrated
  if (!isHydrated) {
    return (
      <header className="w-full border-b bg-background/80 dark:bg-[#18181b]/80 sticky top-0 z-[100] transition-colors duration-300">
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
              <form
                onSubmit={handleSearchSubmit}
                className="w-full max-w-xl relative"
              >
                <div className="relative">
                  <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                  <Input
                    value={searchTerm}
                    onChange={handleSearchChange}
                    onFocus={handleSearchFocus}
                    placeholder="Search..."
                    className="pl-10 pr-10"
                  />
                  {searchTerm && (
                    <Button
                      type="button"
                      variant="ghost"
                      size="icon"
                      onClick={handleClearSearch}
                      className="absolute right-1 top-1/2 transform -translate-y-1/2 h-6 w-6 p-0 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-full"
                    >
                      <X className="h-3 w-3" />
                    </Button>
                  )}
                </div>
              </form>
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
            </div>
          </div>
        </div>
      </header>
    );
  }

  return (
    <header className="w-full border-b bg-background/80 dark:bg-[#18181b]/80 sticky top-0 z-[100] transition-colors duration-300">
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
            <form
              onSubmit={handleSearchSubmit}
              className="w-full max-w-xl relative"
            >
              <div className="relative">
                <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                <Input
                  value={searchTerm}
                  onChange={handleSearchChange}
                  onFocus={handleSearchFocus}
                  placeholder={t("common.searchPlaceholder")}
                  className="pl-10 pr-10"
                />
                {searchTerm && (
                  <Button
                    type="button"
                    variant="ghost"
                    size="icon"
                    onClick={handleClearSearch}
                    className="absolute right-1 top-1/2 transform -translate-y-1/2 h-6 w-6 p-0 hover:bg-gray-100 dark:hover:bg-gray-800 rounded-full"
                  >
                    <X className="h-3 w-3" />
                  </Button>
                )}
              </div>
            </form>
          </div>
          {/* Icon bên phải */}
          <div className="flex items-center gap-2 min-w-[220px] justify-end">
            <Button
              variant="ghost"
              size="icon"
              aria-label="Toggle language"
              title={
                currentLocale === "vi"
                  ? "Chuyển sang English"
                  : "Switch to Vietnamese"
              }
              onClick={() => dispatch(toggleLocale())}
            >
              <Languages className="h-5 w-5" />
            </Button>
            <Button variant="ghost" size="icon" onClick={handleUserClick}>
              <User className="h-5 w-5" />
            </Button>
            <Button variant="ghost" size="icon" onClick={handleWishlistClick}>
              <Heart className="h-5 w-5" />
              {/* {wishlistCount > 0 && (
                <Badge
                  variant="secondary"
                  className="absolute -top-2 -right-2 h-5 w-5 flex items-center justify-center p-1"
                >
                  {wishlistCount}
                </Badge>
              )} */}
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
          </div>
        </div>
        <nav className="w-full flex justify-center"></nav>
      </div>
      <CartDrawer />
      <SearchOverlay />
    </header>
  );
}
