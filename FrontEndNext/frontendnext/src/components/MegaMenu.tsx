"use client";
import {
  useGetRootCategoriesQuery,
  useGetSubCategoriesQuery,
} from "@/lib/services/categoryService";
import Link from "next/link";
import Image from "next/image";
import { Category } from "@/lib/types/category";

type MegaMenuProps = {
  activeIndex: number | null;
  onMouseLeave: () => void;
  top: number;
  arrowX: number | null;
};

export default function MegaMenu({
  activeIndex,
  onMouseLeave,
  top,
  arrowX,
}: MegaMenuProps) {
  const { data: rootCategories } = useGetRootCategoriesQuery();
  const activeCategoryId =
    activeIndex !== null && rootCategories
      ? rootCategories[activeIndex]?.id
      : undefined;
  const { data: subCategories } = useGetSubCategoriesQuery(
    activeCategoryId ?? 0,
    {
      skip: activeIndex === null || activeCategoryId === undefined,
    }
  );

  if (activeIndex === null || !rootCategories) return null;

  const activeCategory = rootCategories[activeIndex];
  const bannerImg = activeCategory?.imageUrl;

  return (
    <div
      className="fixed left-0 right-0 w-full bg-white dark:bg-[#18181b] shadow-lg dark:shadow-xl z-30 flex overflow-x-auto"
      style={{ minHeight: 320, top }}
      onMouseLeave={onMouseLeave}
    >
      {/* Arrow indicator */}
      {arrowX !== null && (
        <div
          style={{
            position: "absolute",
            top: -10,
            left: arrowX - 10,
            width: 0,
            height: 0,
            borderLeft: "10px solid transparent",
            borderRight: "10px solid transparent",
            borderBottom: "10px solid white",
            zIndex: 40,
            filter: "drop-shadow(0 2px 4px rgba(0,0,0,0.08))",
            background: "transparent",
          }}
        />
      )}
      {/* Cột trái: Head categories */}
      <div className="w-1/4 border-r p-6 dark:border-[#23232b]">
        <Link href={`/category/${activeCategory?.slug}-${activeCategory?.id}`}>
          <div className="font-bold text-lg mb-2 dark:text-white hover:underline cursor-pointer">
            {activeCategory?.name}
          </div>
        </Link>
        <div className="text-sm text-muted-foreground dark:text-gray-400">
          {activeCategory?.description}
        </div>
      </div>
      {/* Cột giữa: Subcategories */}
      <div className="w-2/4 p-6">
        <h4 className="font-semibold mb-2 dark:text-white">Danh mục con</h4>
        <ul className="grid grid-cols-2 gap-2">
          {subCategories?.length ? (
            subCategories.map((sub: Category) => (
              <li key={sub.id}>
                <Link
                  href={`/category/${sub.slug}-${sub.id}`}
                  className="hover:underline dark:text-white"
                >
                  {sub.name}
                </Link>
              </li>
            ))
          ) : (
            <li className="text-gray-400 dark:text-gray-500">
              Không có danh mục con
            </li>
          )}
        </ul>
      </div>
      {/* Cột phải: Ảnh category nếu có */}
      <div className="w-1/4 flex items-center justify-center bg-gray-50 dark:bg-[#23232b]">
        {bannerImg && (
          <Image
            src={bannerImg}
            alt="Category"
            width={200}
            height={192}
            className="max-w-full max-h-48 object-cover rounded"
          />
        )}
      </div>
    </div>
  );
}
