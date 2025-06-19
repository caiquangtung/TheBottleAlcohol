"use client";
import Image from "next/image";
import {
  Category,
  useGetRootCategoriesQuery,
} from "@/lib/services/categoryService";
import Link from "next/link";
import { Skeleton } from "@/components/ui/skeleton";

export default function CategorySection() {
  const { data: categories, isLoading, error } = useGetRootCategoriesQuery();
  if (isLoading) {
    return (
      <section className="container mx-auto py-8">
        <div className="flex flex-wrap justify-center gap-6">
          {[...Array(8)].map((_, i) => (
            <div key={i} className="flex flex-col items-center gap-2">
              <Skeleton className="h-16 w-16 rounded-full" />
              <Skeleton className="h-4 w-20" />
            </div>
          ))}
        </div>
      </section>
    );
  }

  if (error) {
    return (
      <section className="container mx-auto py-8">
        <div className="text-red-500 text-center">
          Failed to load categories
        </div>
      </section>
    );
  }
  return (
    <section className="container mx-auto py-8">
      <div className="flex flex-wrap justify-center gap-6">
        {categories?.map((category) => (
          <Link
            href={`/category/${category.slug}-${category.id}`}
            key={category.id}
            className="flex flex-col items-center gap-2 hover:opacity-80 transition-opacity"
          >
            <div className="relative w-[120px] h-[120px] rounded-full overflow-hidden bg-muted dark:bg-[#23232b]">
              <Image
                src={category.imageUrl || "/category.png"}
                alt={category.name}
                fill
                className="object-cover"
              />
            </div>
            <span className="text-sm font-medium">{category.name}</span>
          </Link>
        ))}
      </div>
    </section>
  );
}
