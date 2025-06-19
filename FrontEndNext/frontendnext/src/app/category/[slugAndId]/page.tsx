"use client";
import CategoryDescription from "@/components/CategoryDescription";
import { useGetCategoryByIdQuery } from "@/lib/services/categoryService";
import React from "react";

function extractId(slugAndId?: string) {
  if (!slugAndId) return NaN;
  const parts = slugAndId.split("-");
  return parts.length > 1 ? Number(parts[parts.length - 1]) : NaN;
}

export default function CategoryDetailPage({
  params,
}: {
  params: Promise<{ slugAndId?: string }>;
}) {
  const { slugAndId } = React.use(params);
  const id = extractId(slugAndId);
  const {
    data: category,
    isLoading,
    isError,
  } = useGetCategoryByIdQuery(id, { skip: isNaN(id) });

  if (!slugAndId || isNaN(id)) {
    return (
      <div className="container mx-auto py-8 px-4 text-red-500">
        Invalid category URL
      </div>
    );
  }
  if (isLoading)
    return <div className="container mx-auto py-8 px-4">Loading...</div>;
  if (isError || !category)
    return (
      <div className="container mx-auto py-8 px-4 text-red-500">
        Category not found
      </div>
    );

  return (
    <section className="w-full mb-8">
      <CategoryDescription
        title={category.metaTitle || category.name}
        metaDescription={category.metaDescription}
        description={category.description}
        imageUrl={category.imageUrl}
        breadcrumb={[
          { name: "Home", href: "/" },
          { name: category.metaTitle || category.name, href: "#" },
        ]}
      />
      {/* ...rest of category detail page... */}
    </section>
  );
}
