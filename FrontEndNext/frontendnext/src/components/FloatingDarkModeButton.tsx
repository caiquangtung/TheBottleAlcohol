"use client";
import { Button } from "./ui/button";
import { Sun, Moon } from "lucide-react";
import { useSelector } from "react-redux";
import { RootState } from "../lib/store";
import { useAppDispatch } from "../lib/store/hooks";
import { toggleDark } from "../lib/features/theme/themeSlice";
import { useState, useEffect } from "react";

export default function FloatingDarkModeButton() {
  const [isHydrated, setIsHydrated] = useState(false);
  const isDark = useSelector((state: RootState) => state.theme.isDark);
  const dispatch = useAppDispatch();

  // Hydration check
  useEffect(() => {
    setIsHydrated(true);
  }, []);

  // Don't render until hydrated
  if (!isHydrated) {
    return null;
  }

  return (
    <div className="fixed bottom-6 right-6 z-50">
      <Button
        variant="default"
        size="icon"
        className="w-12 h-12 rounded-full shadow-lg hover:shadow-xl transition-all duration-300 bg-primary hover:bg-primary/90"
        aria-label="Toggle dark mode"
        onClick={() => dispatch(toggleDark())}
      >
        {isDark ? <Sun className="h-6 w-6" /> : <Moon className="h-6 w-6" />}
      </Button>
    </div>
  );
}
