"use client";
import React, { useRef } from "react";
import { useGetRootCategoriesQuery } from "@/lib/services/categoryService";

type HeadCategoryBarProps = {
  onHoverCategory: (idx: number | null, centerX?: number) => void;
  innerRef?: React.Ref<HTMLDivElement>;
};

export default function HeadCategoryBar({
  onHoverCategory,
  innerRef,
}: HeadCategoryBarProps) {
  const { data: categories, isLoading } = useGetRootCategoriesQuery();
  const itemRefs = useRef<(HTMLDivElement | null)[]>([]);

  if (isLoading) return null;

  return (
    <nav
      ref={innerRef}
      className="flex justify-center gap-3 px-8 py-2 bg-white dark:bg-[#18181b] shadow z-20 relative border-b border-gray-300 dark:border-[#23232b]"
    >
      {categories?.map((cat, idx) => (
        <div
          key={cat.id}
          ref={(el) => {
            itemRefs.current[idx] = el;
          }}
          className="font-semibold cursor-pointer transition relative px-4 py-2 border-b-2 border-transparent hover:border-primary hover:text-primary bg-white dark:bg-[#18181b] dark:text-white"
          onMouseEnter={() => {
            const rect = itemRefs.current[idx]?.getBoundingClientRect();
            const centerX = rect ? rect.left + rect.width / 2 : undefined;
            onHoverCategory(idx, centerX);
          }}
        >
          {cat.name}
        </div>
      ))}
    </nav>
  );
}
