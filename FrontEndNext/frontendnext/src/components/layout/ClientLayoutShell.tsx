"use client";
import { useLayoutEffect, useRef, useState, useEffect } from "react";
import HeadCategoryBar from "../HeadCategoryBar";
import MegaMenu from "../MegaMenu";
import { useAppDispatch, useAppSelector } from "@/lib/store/hooks";
import { useGetCartQuery } from "@/lib/services/cartService";
import { setCartData } from "@/lib/features/cart/cartSlice";

export default function ClientLayoutShell({
  children,
}: {
  children: React.ReactNode;
}) {
  const [activeIndex, setActiveIndex] = useState<number | null>(null);
  const [arrowX, setArrowX] = useState<number | null>(null);
  const headCatRef = useRef<HTMLDivElement>(null);
  const [menuTop, setMenuTop] = useState(0);

  const dispatch = useAppDispatch();
  const isLoggedIn = useAppSelector((state) => state.auth.isAuthenticated);
  const user = useAppSelector((state) => state.auth.user);
  const userId = user?.id;

  const { data: cartData } = useGetCartQuery(
    typeof userId === "number" ? userId : -1,
    {
      skip: !isLoggedIn || typeof userId !== "number",
    }
  );

  useEffect(() => {
    if (cartData) {
      dispatch(setCartData(cartData));
    }
  }, [cartData, dispatch]);

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
