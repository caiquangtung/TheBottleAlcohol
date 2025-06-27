import React from "react";
import { Product } from "@/lib/types/product";
import ProductCard from "./ProductCard";

interface ProductGridProps {
  products: Product[];
}

const ProductGrid: React.FC<ProductGridProps> = ({ products }) => {
  if (!products || products.length === 0) {
    return (
      <div className="text-center py-16 px-4 bg-gray-50 rounded-lg">
        <h3 className="text-2xl font-semibold text-gray-800">
          No Products Found
        </h3>
        <p className="mt-2 text-gray-500">
          We couldn&apos;t find any products matching your current filters.
        </p>
        <p className="mt-1 text-gray-500">
          Try adjusting your search or filter criteria.
        </p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
      {products.map((product) => (
        <ProductCard key={product.id} product={product} />
      ))}
    </div>
  );
};

export default ProductGrid;
