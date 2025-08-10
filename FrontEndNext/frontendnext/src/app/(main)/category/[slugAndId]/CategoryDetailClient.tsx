"use client";
import CategoryDescription from "@/components/CategoryDescription";
import {
  useGetCategoryByIdQuery,
  useGetSubCategoriesQuery,
} from "@/lib/services/categoryService";
import { Category } from "@/lib/types/category";
import React, { useState, useEffect } from "react";
import {
  useGetProductsQuery,
  type ProductFilter as FilterState,
} from "@/lib/services/productService";
import Link from "next/link";
import ProductGrid from "@/components/ProductGrid";
import { useGetAllBrandsQuery } from "@/lib/services/brandService";
import { FilterSortSheet } from "@/components/FilterSortSheet";
import { Skeleton } from "@/components/ui/skeleton";
import { PaginationControls } from "@/components/ui/Pagination";

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

export default function CategoryDetailClient({
  categoryId,
}: {
  categoryId: number;
}) {
  const [filters, setFilters] = useState<FilterState>({
    pageNumber: 1,
    pageSize: 12,
    categoryId: categoryId,
  });

  useEffect(() => {
    if (categoryId && !isNaN(categoryId)) {
      setFilters(() => ({
        categoryId: categoryId,
        pageNumber: 1,
        pageSize: 12,
      }));
    }
  }, [categoryId]);

  const { data: category, isLoading: isLoadingCategory } =
    useGetCategoryByIdQuery(categoryId, { skip: isNaN(categoryId) });
  const { data: siblingCategories } = useGetSubCategoriesQuery(
    category?.parentId ?? 0,
    { skip: !category?.parentId }
  );
  const { data: childCategories } = useGetSubCategoriesQuery(categoryId, {
    skip: isNaN(categoryId),
  });
  const { data: pagedProducts, isLoading: isLoadingProducts } =
    useGetProductsQuery(filters, { skip: !filters.categoryId });
  const { data: brands = [], isLoading: isLoadingBrands } =
    useGetAllBrandsQuery();

  const products = pagedProducts?.items ?? [];
  const totalProducts = pagedProducts?.totalRecords ?? 0;
  const totalPages = pagedProducts?.totalPages ?? 1;

  const handlePageChange = (newPage: number) => {
    setFilters((prevFilters) => ({
      ...prevFilters,
      pageNumber: newPage,
    }));
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

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
        <SubCategoryNavigation currentCategory={category}>
          {displayNavCategories ?? []}
        </SubCategoryNavigation>

        {/* Main Content */}
        <div className="flex justify-between items-center mb-6 mt-8">
          <FilterSortSheet
            filters={filters}
            setFilters={setFilters}
            brands={brands}
            totalProducts={totalProducts}
            isLoadingBrands={isLoadingBrands}
          />
        </div>

        {/* Products Grid */}
        {isLoadingProducts ? (
          <ProductGridSkeleton />
        ) : (
          <>
            <ProductGrid products={products} />
            {totalPages > 1 && (
              <div className="mt-8 flex justify-center">
                <PaginationControls
                  currentPage={filters.pageNumber || 1}
                  totalPages={totalPages}
                  onPageChange={handlePageChange}
                />
              </div>
            )}
          </>
        )}
      </div>
    </section>
  );
}
