"use client";
import React from "react";
import { ProductFilter } from "@/lib/types/product";
import { useGetAllBrandsQuery } from "@/lib/services/brandService";
import { Brand } from "@/lib/types/brand";
import { Button } from "./ui/button";
import { Input } from "./ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { Skeleton } from "./ui/skeleton";
import { useGetAllCategoriesQuery } from "@/lib/services/categoryService";
import { useState, useEffect } from "react";

interface ProductFilterProps {
  filters: ProductFilter;
  setFilters: React.Dispatch<React.SetStateAction<ProductFilter>>;
  brands: Brand[];
  isLoadingBrands: boolean;
}

const ProductFilterComponent: React.FC<ProductFilterProps> = ({
  filters,
  setFilters,
  brands,
  isLoadingBrands,
}) => {
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFilters((prev) => ({
      ...prev,
      [name]: value ? Number(value) : undefined,
    }));
  };

  const handleBrandChange = (brandId: number) => {
    setFilters((prev) => ({
      ...prev,
      brandId: prev.brandId === brandId ? undefined : brandId,
    }));
  };

  const handleSortChange = (value: string) => {
    if (!value) {
      setFilters((prev) => ({
        ...prev,
        sortBy: undefined,
        sortOrder: undefined,
      }));
      return;
    }
    const [sortBy, sortOrder] = value.split("_");
    setFilters((prev) => ({
      ...prev,
      sortBy,
      sortOrder: sortOrder as "asc" | "desc",
    }));
  };

  return (
    <div className="space-y-4">
      <h3 className="text-2xl font-bold tracking-tight">Filter & Sort</h3>
      <Accordion
        type="multiple"
        defaultValue={["sort", "price", "brand"]}
        className="w-full"
      >
        {/* Sort By */}
        <AccordionItem value="sort">
          <AccordionTrigger className="text-lg font-semibold">
            Sort By
          </AccordionTrigger>
          <AccordionContent>
            <Select
              onValueChange={handleSortChange}
              defaultValue={
                filters.sortBy ? `${filters.sortBy}_${filters.sortOrder}` : ""
              }
            >
              <SelectTrigger>
                <SelectValue placeholder="Default" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="price_asc">Price: Low to High</SelectItem>
                <SelectItem value="price_desc">Price: High to Low</SelectItem>
                <SelectItem value="name_asc">Name: A-Z</SelectItem>
                <SelectItem value="name_desc">Name: Z-A</SelectItem>
              </SelectContent>
            </Select>
          </AccordionContent>
        </AccordionItem>

        {/* Price */}
        <AccordionItem value="price">
          <AccordionTrigger className="text-lg font-semibold">
            Price
          </AccordionTrigger>
          <AccordionContent>
            <div className="flex items-center space-x-2">
              <Input
                type="number"
                name="minPrice"
                placeholder="Min"
                value={filters.minPrice || ""}
                onChange={handleInputChange}
                className="w-full"
              />
              <span>-</span>
              <Input
                type="number"
                name="maxPrice"
                placeholder="Max"
                value={filters.maxPrice || ""}
                onChange={handleInputChange}
                className="w-full"
              />
            </div>
          </AccordionContent>
        </AccordionItem>

        {/* Brand */}
        <AccordionItem value="brand">
          <AccordionTrigger className="text-lg font-semibold">
            Brand
          </AccordionTrigger>
          <AccordionContent>
            {isLoadingBrands ? (
              <div className="space-y-2">
                <Skeleton className="h-8 w-full" />
                <Skeleton className="h-8 w-full" />
                <Skeleton className="h-8 w-full" />
              </div>
            ) : (
              <div className="space-y-2 max-h-60 overflow-y-auto">
                {brands.map((brand: Brand) => (
                  <Button
                    key={brand.id}
                    variant={
                      filters.brandId === brand.id ? "secondary" : "ghost"
                    }
                    onClick={() => handleBrandChange(brand.id)}
                    className="w-full justify-start font-normal"
                  >
                    {brand.name}
                  </Button>
                ))}
              </div>
            )}
          </AccordionContent>
        </AccordionItem>
      </Accordion>
    </div>
  );
};

export default ProductFilterComponent;
