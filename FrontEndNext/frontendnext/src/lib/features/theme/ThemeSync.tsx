"use client";
import { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { RootState } from "../../store/index";

export default function ThemeSync() {
  const isDark = useSelector((state: RootState) => state.theme.isDark);
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    if (isDark) {
      document.documentElement.classList.add("dark");
      localStorage.setItem("theme", "dark");
    } else {
      document.documentElement.classList.remove("dark");
      localStorage.setItem("theme", "light");
    }
    setMounted(true);
  }, [isDark]);

  useEffect(() => {
    if (mounted) {
      document.body.classList.remove("invisible");
    } else {
      document.body.classList.add("invisible");
    }
  }, [mounted]);

  return null;
}
