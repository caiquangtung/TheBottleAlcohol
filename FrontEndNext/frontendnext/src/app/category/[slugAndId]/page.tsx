"use client";
import CategoryDescription from "@/components/CategoryDescription";
import {
  useGetCategoryByIdQuery,
  useGetSubCategoriesQuery,
  Category,
} from "@/lib/services/categoryService";
import React, { useState, useEffect } from "react";
import {
  useGetProductsQuery,
  ProductFilter as FilterState,
  Product,
  PagedResult,
} from "@/lib/services/productService";
import Link from "next/link";
import ProductGrid from "@/components/ProductGrid";
import ProductFilterComponent from "@/components/ProductFilter";
import { useGetAllBrandsQuery } from "@/lib/services/brandService";
import { FilterSortSheet } from "@/components/FilterSortSheet";
import { Button } from "@/components/ui/button";
import { useParams } from "next/navigation";
import { Skeleton } from "@/components/ui/skeleton";

function extractId(slugAndId?: string) {
  if (!slugAndId) return NaN;
  const parts = slugAndId.split("-");
  return parseInt(parts[parts.length - 1], 10);
}

const ProductGridSkeleton = () => (
  <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
    {Array.from({ length: 9 }).map((_, i) => (
      <div key={i} className="space-y-2">
        <Skeleton className="h-64 w-full" />
        <Skeleton className="h-4 w-3/4" />
        <Skeleton className="h-4 w-1/2" />
      </div>
    ))}
  </div>
);

const SubCategoryNavigation = ({
  currentCategory,
  children,
}: {
  currentCategory: Category | null;
  children: Category[];
}) => {
  if (!currentCategory) {
    return null;
  }

  const hasParent =
    currentCategory.parentId &&
    currentCategory.parentName &&
    currentCategory.parentSlug;

  return (
    <div className="flex flex-wrap items-center gap-2 my-4 border-b pb-4">
      {hasParent && (
        <Link
          href={`/category/${currentCategory.parentSlug}-${currentCategory.parentId}`}
          className="flex items-center gap-1 text-sm text-gray-600 hover:text-black"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-4 w-4"
            fill="none"
            viewBox="0 0 24 24"
            stroke="currentColor"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              strokeWidth={2}
              d="M15 19l-7-7 7-7"
            />
          </svg>
          {`Back to ${currentCategory.parentName}`}
        </Link>
      )}
      {hasParent && <div className="h-4 w-px bg-gray-300"></div>}

      <Link
        key={`shop-all-${currentCategory.id}`}
        href={`/category/${currentCategory.slug}-${currentCategory.id}`}
        className="px-3 py-1 bg-gray-800 text-white hover:bg-black rounded-full text-sm font-semibold"
      >
        {`Shop all ${currentCategory.name}`}
      </Link>

      {children.map((c: Category) => (
        <Link
          key={c.id}
          href={`/category/${c.slug}-${c.id}`}
          className="px-3 py-1 bg-gray-100 hover:bg-gray-200 rounded-full text-sm"
        >
          {c.name}
        </Link>
      ))}
    </div>
  );
};

export default function CategoryPage() {
  const params = useParams();
  const slugAndId = params.slugAndId as string;
  const id = extractId(slugAndId);

  const [filters, setFilters] = useState<FilterState>({
    pageNumber: 1,
    pageSize: 12,
    categoryId: id,
  });

  useEffect(() => {
    if (id && !isNaN(id)) {
      setFilters((prev) => ({
        categoryId: id,
        pageNumber: 1,
        pageSize: 12,
      }));
    }
  }, [id]);

  const { data: category, isLoading: isLoadingCategory } =
    useGetCategoryByIdQuery(id, { skip: isNaN(id) });
  const { data: siblingCategories } = useGetSubCategoriesQuery(
    category?.parentId ?? 0,
    { skip: !category?.parentId }
  );
  const { data: childCategories } = useGetSubCategoriesQuery(id, {
    skip: isNaN(id),
  });
  const {
    data: pagedProducts,
    isFetching: isFetchingProducts,
    isLoading: isLoadingProducts,
  } = useGetProductsQuery(filters, { skip: !filters.categoryId });
  const { data: brands = [], isLoading: isLoadingBrands } =
    useGetAllBrandsQuery();

  const products = pagedProducts?.items ?? [];
  const totalProducts = pagedProducts?.totalRecords ?? 0;

  if (isLoadingCategory) {
    return (
      <div className="container mx-auto px-4 md:px-6 py-8">Loading...</div>
    );
  }

  if (!category) {
    return (
      <div className="container mx-auto px-4 md:px-6 py-8">
        Category not found.
      </div>
    );
  }

  const hasParent =
    category.parentId && category.parentName && category.parentSlug;
  const displayNavCategories = hasParent ? siblingCategories : childCategories;

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

      <div className="container mx-auto px-4 md:px-6 py-8">
        {/* Sub Category Navigation */}
        <SubCategoryNavigation
          currentCategory={category}
          children={displayNavCategories ?? []}
        />

        {/* Main Content */}
        <div className="flex justify-between items-center mb-6 mt-8">
          <h2 className="text-xl sm:text-2xl font-bold tracking-tight">
            {totalProducts} results
          </h2>
          <FilterSortSheet
            filters={filters}
            setFilters={setFilters}
            brands={brands}
            totalProducts={totalProducts}
            isLoadingBrands={isLoadingBrands}
          />
        </div>

        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          {/* Filters - Hidden on mobile */}
          <div className="hidden md:block">
            <ProductFilterComponent
              filters={filters}
              setFilters={setFilters}
              brands={brands}
              isLoadingBrands={isLoadingBrands}
            />
          </div>

          {/* Products Grid */}
          <div className="md:col-span-3">
            {isFetchingProducts || isLoadingProducts ? (
              <ProductGridSkeleton />
            ) : (
              <ProductGrid products={products} />
            )}
          </div>
        </div>
      </div>
    </section>
  );
}
