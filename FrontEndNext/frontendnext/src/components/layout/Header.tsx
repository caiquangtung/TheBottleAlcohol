"use client";
import Image from "next/image";
import { Input } from "../ui/input";
import { Button } from "../ui/button";
import { Heart, ShoppingCart, Sun, Moon, User } from "lucide-react";
import { useSelector } from "react-redux";
import { RootState } from "../../lib/store";
import { useAppDispatch, useAppSelector } from "../../lib/store/hooks";
import { toggleDark } from "../../lib/features/theme/themeSlice";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function Header() {
  const isDark = useSelector((state: RootState) => state.theme.isDark);
  const dispatch = useAppDispatch();
  const router = useRouter();
  const user = useAppSelector((state) => state.auth.user);

  const handleUserClick = () => {
    if (user) {
      router.push("/profile");
    } else {
      router.push("/login");
    }
  };

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
            <Button variant="ghost" size="icon">
              <Heart className="h-5 w-5" />
            </Button>
            <Button variant="ghost" size="icon">
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
    </header>
  );
}
