import React from "react";
import Link from "next/link";
import Image from "next/image";
import { Product } from "@/lib/services/productService";
import { Button } from "./ui/button";
import { Card } from "./ui/card";
import { Badge } from "./ui/badge";
import { Heart } from "lucide-react";

interface ProductCardProps {
  product: Product;
  className?: string;
}

const formatPrice = (price: number) => {
  return new Intl.NumberFormat("en-GB", {
    style: "currency",
    currency: "GBP",
  }).format(price);
};

const formatVolume = (volume: number) => {
  return `${volume.toFixed(2)}CL`;
};

const formatABV = (abv: number) => {
  return `${abv.toFixed(1)}%`;
};

const ProductCard: React.FC<ProductCardProps> = ({ product, className }) => {
  return (
    <Card
      className={`relative flex flex-col overflow-hidden dark:bg-[#23232b] ${className}`}
    >
      <div className="pt-4 px-4 flex flex-col h-full">
        {product.stockQuantity < 10 && (
          <Badge className="absolute top-2 left-2 dark:bg-[#3a2a3a] dark:text-[#f96d8d] !px-3 !py-1 text-xs font-semibold tracking-wide z-10">
            LIMITED STOCK
          </Badge>
        )}
        <Button
          variant="ghost"
          size="icon"
          className="absolute top-2 right-2 z-10"
        >
          <Heart className="h-5 w-5" />
        </Button>
        <Link
          href={`/product/${product.slug}-${product.id}`}
          className="flex flex-col flex-grow"
        >
          <div className="relative w-full aspect-[3/4] flex-shrink-0">
            <Image
              src={product.imageUrl || "/product.png"}
              alt={product.name}
              fill
              sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
              className="object-contain group-hover:scale-105 transition-transform duration-300"
            />
          </div>

          <div className="mt-2 flex flex-col flex-grow space-y-1">
            <div className="flex items-center justify-between">
              <span className="text-xs text-muted-foreground uppercase tracking-wide">
                {product.brandName}
              </span>
              <span className="text-xs text-muted-foreground uppercase">
                {product.categoryName}
              </span>
            </div>
            <h3 className="font-bold text-base line-clamp-2">{product.name}</h3>
            <div className="flex items-center justify-between w-full">
              <span className="font-bold text-lg">
                {formatPrice(product.price)}
              </span>
              <div className="flex items-center gap-1 text-sm">
                {[...Array(5)].map((_, idx) => (
                  <span key={idx} className="text-gray-300">
                    ★
                  </span>
                ))}
              </div>
            </div>
            <div className="flex items-center gap-1 text-xs text-muted-foreground">
              <span>{formatVolume(product.volume)}</span>
              <span>•</span>
              <span>{formatABV(product.alcoholContent)}</span>
              <span>•</span>
              <span>{product.origin}</span>
            </div>
          </div>
        </Link>
        <Button
          size="lg"
          className="w-full mt-4 border-2 border-black bg-transparent text-black dark:bg-[#f96d8d] dark:text-black dark:border-white font-bold tracking-wide hover:bg-black hover:text-white dark:hover:bg-white dark:hover:text-black rounded-none rounded-b-lg"
        >
          ADD TO CART
        </Button>
      </div>
    </Card>
  );
};

export default ProductCard;
