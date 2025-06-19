"use client";
import Image from "next/image";
import { Card } from "../ui/card";
import { Badge } from "../ui/badge";
import { Button } from "../ui/button";
import { Heart } from "lucide-react";
import { useGetFeaturedProductsQuery } from "@/lib/services/productService";
import { Skeleton } from "../ui/skeleton";

export default function ProductSection() {
  const {
    data: products,
    isLoading,
    error,
    isError,
    isSuccess,
  } = useGetFeaturedProductsQuery(undefined, {
    refetchOnMountOrArgChange: true,
  });

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

  if (isLoading) {
    return (
      <section className="container mx-auto py-8">
        <h2 className="text-2xl md:text-3xl font-bold text-center mb-2">
          OUR TOP PICKS OF THE WEEK
        </h2>
        <p className="text-center text-muted-foreground mb-8">
          Loading featured products...
        </p>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <Card className="md:col-span-1 flex flex-col justify-between min-h-[520px] relative dark:bg-[#18181b]">
            <Skeleton className="w-full h-full" />
          </Card>
          <div className="md:col-span-2 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {[...Array(6)].map((_, i) => (
              <Card
                key={i}
                className="relative flex flex-col h-full dark:bg-[#23232b]"
              >
                <Skeleton className="w-full h-60" />
                <div className="p-4">
                  <Skeleton className="h-4 w-20 mb-2" />
                  <Skeleton className="h-6 w-32 mb-2" />
                  <Skeleton className="h-4 w-24 mb-2" />
                  <Skeleton className="h-8 w-full" />
                </div>
              </Card>
            ))}
          </div>
        </div>
      </section>
    );
  }

  if (isError) {
    return (
      <section className="container mx-auto py-8">
        <h2 className="text-2xl md:text-3xl font-bold text-center mb-2">
          OUR TOP PICKS OF THE WEEK
        </h2>
        <p className="text-center text-muted-foreground mb-8">
          Failed to load products. Please try again later.
        </p>
        <div className="text-center">
          <Button
            onClick={() => window.location.reload()}
            variant="outline"
            className="mt-4"
          >
            Retry
          </Button>
        </div>
      </section>
    );
  }

  if (!products || products.length === 0) {
    return (
      <section className="container mx-auto py-8">
        <h2 className="text-2xl md:text-3xl font-bold text-center mb-2">
          OUR TOP PICKS OF THE WEEK
        </h2>
        <p className="text-center text-muted-foreground mb-8">
          No products available at the moment.
        </p>
      </section>
    );
  }

  return (
    <section className="container mx-auto py-8 px-4 md:px-6 lg:px-8">
      <div className="max-w-[1280px] mx-auto">
        <h2 className="text-2xl md:text-3xl font-bold text-center mb-2">
          WIMBLEDON READY
        </h2>
        <p className="text-center text-muted-foreground mb-8 max-w-3xl mx-auto">
          It's not Wimbledon without strawberries — and we've bottled the vibe.
          From berry-infused gins to juicy liqueurs and sparkling sips, these
          strawberry-flavoured drinks are serving summer in every pour. Sweet,
          fruity, and seriously good — no racket required.
        </p>
        <div className="grid grid-cols-12 gap-6">
          {/* Left: Promo card - spans 5 columns */}
          <Card
            className="col-span-12 md:col-span-5 flex flex-col justify-between bg-cover bg-center relative dark:bg-[#18181b] min-h-[600px] row-span-2"
            style={{
              backgroundImage: "url(/promo-hennessy.png)",
            }}
          >
            <a
              href="/collections/new-arrivals"
              title="GAME, SIP, MATCH"
              className="absolute inset-0 w-full h-full z-10"
              tabIndex={-1}
              aria-label="GAME, SIP, MATCH"
            ></a>
            <div className="relative z-20 flex flex-col h-full justify-end p-8">
              <h3 className="text-4xl font-bold text-white mb-6 drop-shadow-lg">
                GAME, SIP, MATCH
              </h3>
              <Button
                variant="secondary"
                className="w-fit dark:bg-[#f96d8d] dark:text-black"
              >
                SHOP NOW
              </Button>
            </div>
          </Card>

          {/* Right: Product grid - spans 7 columns */}
          <div className="col-span-12 md:col-span-7 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-x-6">
            {products.slice(0, 6).map((product, index) => (
              <Card
                key={product.Id}
                className={`relative flex flex-col overflow-hidden dark:bg-[#23232b] h-[55vh] ${
                  index < 3 ? "mb-6" : "mb-0"
                }`}
              >
                <div className="pt-4 px-4 flex flex-col h-full">
                  {product.StockQuantity < 10 && (
                    <Badge className="absolute top-2 left-2 dark:bg-[#3a2a3a] dark:text-[#f96d8d] !px-3 !py-1 text-xs font-semibold tracking-wide">
                      LIMITED STOCK
                    </Badge>
                  )}
                  <Button
                    variant="ghost"
                    size="icon"
                    className="absolute top-2 right-2"
                  >
                    <Heart className="h-5 w-5" />
                  </Button>

                  {/* Product Image */}
                  <div className="relative w-full aspect-[3/4] flex-shrink-0">
                    <Image
                      src={product.ImageUrl || "/product.png"}
                      alt={product.Name}
                      fill
                      className="object-contain"
                      sizes="(max-width: 640px) 100vw, (max-width: 1024px) 50vw, 33vw"
                    />
                  </div>

                  {/* Product Info */}
                  <div className="mt-2 flex flex-col flex-grow space-y-1">
                    <div className="flex items-center justify-between">
                      <span className="text-xs text-muted-foreground uppercase tracking-wide">
                        {product.BrandName}
                      </span>
                      <span className="text-xs text-muted-foreground uppercase">
                        {product.CategoryName}
                      </span>
                    </div>
                    <h3 className="font-bold text-base line-clamp-2">
                      {product.Name}
                    </h3>
                    <div className="flex items-center justify-between w-full">
                      <span className="font-bold text-lg">
                        {formatPrice(product.Price)}
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
                      <span>{formatVolume(product.Volume)}</span>
                      <span>•</span>
                      <span>{formatABV(product.AlcoholContent)}</span>
                      <span>•</span>
                      <span>{product.Origin}</span>
                    </div>
                  </div>

                  {/* Add to Cart Button */}
                  <Button
                    size="lg"
                    className="w-full mt-4 border-2 border-black bg-transparent text-black dark:bg-[#f96d8d] dark:text-black dark:border-white font-bold tracking-wide hover:bg-black hover:text-white dark:hover:bg-white dark:hover:text-black rounded-none rounded-b-lg"
                  >
                    ADD TO CART
                  </Button>
                </div>
              </Card>
            ))}
          </div>
        </div>
      </div>
    </section>
  );
}
