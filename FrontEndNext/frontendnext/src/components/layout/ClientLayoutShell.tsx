"use client";
import { useLayoutEffect, useRef, useState } from "react";
import HeadCategoryBar from "../HeadCategoryBar";
import MegaMenu from "../MegaMenu";

export default function ClientLayoutShell({
  children,
}: {
  children: React.ReactNode;
}) {
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
    <div className="flex-1 flex flex-col">
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
      <main className="flex-1 flex flex-col">{children}</main>
    </div>
  );
}
