"use client";
import { useRef, useState, useLayoutEffect } from "react";
import Header from "../components/layout/Header";
import Banner from "../components/home/Banner";
import CategorySection from "../components/home/CategorySection";
import ProductSection from "../components/home/ProductSection";
import HeadCategoryBar from "../components/HeadCategoryBar";
import MegaMenu from "../components/MegaMenu";

export default function Home() {
  const [activeIndex, setActiveIndex] = useState<number | null>(null);
  const [arrowX, setArrowX] = useState<number | null>(null);
  const headCatRef = useRef<HTMLDivElement>(null);
  const [menuTop, setMenuTop] = useState(0);

  const updateMenuTop = () => {
    if (headCatRef.current) {
      const rect = headCatRef.current.getBoundingClientRect();
      setMenuTop(rect.bottom + window.scrollY);
    }
  };

  useLayoutEffect(() => {
    updateMenuTop();
  }, [activeIndex]);

  return (
    <div className="min-h-screen flex flex-col bg-background text-foreground transition-colors duration-300">
      <Header />
      <div onMouseLeave={() => setActiveIndex(null)} className="relative">
        <HeadCategoryBar
          onHoverCategory={(idx, centerX) => {
            setActiveIndex(idx);
            setArrowX(centerX ?? null);
          }}
          innerRef={headCatRef}
        />
        <MegaMenu
          activeIndex={activeIndex}
          onMouseLeave={() => {}}
          top={menuTop}
          arrowX={arrowX}
        />
      </div>
      <Banner />
      <CategorySection />
      <ProductSection />
    </div>
  );
}
