import React from "react";
import Link from "next/link";
import Image from "next/image";
import { Product } from "@/lib/services/productService";
import { Button } from "./ui/button";

interface ProductCardProps {
  product: Product;
}

const ProductCard: React.FC<ProductCardProps> = ({ product }) => {
  return (
    <div className="border rounded-lg overflow-hidden group">
      <Link href={`/product/${product.slug}-${product.id}`} className="block">
        <div className="relative w-full h-64 bg-gray-200">
          {product.imageUrl && (
            <Image
              src={product.imageUrl}
              alt={product.name}
              layout="fill"
              objectFit="cover"
              className="group-hover:scale-105 transition-transform duration-300"
            />
          )}
        </div>
        <div className="p-4">
          <p className="text-sm text-gray-500">{product.brandName}</p>
          <h3 className="text-lg font-semibold truncate mt-1">
            {product.name}
          </h3>
          <p className="text-xl font-bold mt-2">${product.price.toFixed(2)}</p>
        </div>
      </Link>
      <div className="p-4 border-t">
        <Button className="w-full">Add to Cart</Button>
      </div>
    </div>
  );
};

export default ProductCard;
