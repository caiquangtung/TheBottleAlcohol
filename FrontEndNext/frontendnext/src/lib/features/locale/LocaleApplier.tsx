"use client";
import { useEffect } from "react";
import { useSelector } from "react-redux";
import { RootState } from "@/lib/store";

export default function LocaleApplier() {
  const locale = useSelector((s: RootState) => s.locale.current);

  useEffect(() => {
    try {
      const html = document.documentElement;
      html.setAttribute("lang", locale);
    } catch {}
  }, [locale]);

  return null;
}
