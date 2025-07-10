"use client";
import React, { useState } from "react";
import { useGetProductByIdQuery } from "@/lib/services/productService";
import { Product } from "@/lib/types/product";
import { Skeleton } from "@/components/ui/skeleton";
import Image from "next/image";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import {
  Globe,
  Tag,
  GlassWater,
  Percent,
  Minus,
  Plus,
  Wine,
  Clock,
  ChevronRight,
} from "lucide-react";
import { Separator } from "@/components/ui/separator";
import { useAppSelector, useAppDispatch } from "@/lib/store/hooks";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import { addItem } from "@/lib/features/cart/cartSlice";
import { useSyncCartMutation } from "@/lib/services/cartService";
import ReviewSection from "@/components/ReviewSection";
import { useGetProductsByBrandQuery } from "@/lib/services/productService";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetClose,
} from "@/components/ui/sheet";

const ProductPageSkeleton = () => (
  <div className="container mx-auto px-4 md:px-6 py-8">
    {/* Skeleton for Breadcrumb */}
    <Skeleton className="h-4 w-1/3 mb-6" />

    <div className="grid md:grid-cols-12 gap-8 lg:gap-12">
      {/* Left Column: Info List Skeleton */}
      <div className="hidden md:block md:col-span-3 lg:col-span-2">
        <div className="space-y-6">
          {Array.from({ length: 5 }).map((_, i) => (
            <div key={i} className="space-y-2">
              <Skeleton className="h-3 w-12" />
              <Skeleton className="h-4 w-24" />
            </div>
          ))}
        </div>
      </div>

      {/* Center Column: Image Skeleton */}
      <div className="md:col-span-5 lg:col-span-6">
        <Skeleton className="w-full aspect-square rounded-lg" />
      </div>

      {/* Right Column: Details Skeleton */}
      <div className="md:col-span-4 lg:col-span-4 space-y-4">
        <Skeleton className="h-8 w-3/4" />
        <Skeleton className="h-6 w-1/4" />
        <Skeleton className="h-20 w-full" />
        <Skeleton className="h-12 w-full" />
        <Skeleton className="h-4 w-1/4 mt-2" />
      </div>
    </div>
  </div>
);

const ProductInfoList = ({ product }: { product: Product }) => {
  type InfoItem = { icon: React.JSX.Element; label: string; value: string };
  const infoItems: InfoItem[] = [
    {
      icon: <Tag className="h-5 w-5" />,
      label: "Brand",
      value: product.brandName,
    },
    {
      icon: <Globe className="h-5 w-5" />,
      label: "Country",
      value: product.origin,
    },
    {
      icon: <GlassWater className="h-5 w-5" />,
      label: "Size",
      value: `${product.volume}cl`,
    },
    {
      icon: <Percent className="h-5 w-5" />,
      label: "ABV",
      value: `${product.alcoholContent}%`,
    },
    ...(product.flavor
      ? [
          {
            icon: <Wine className="h-5 w-5" />,
            label: "Flavor",
            value: product.flavor,
          },
        ]
      : []),
    ...(product.age
      ? [
          {
            icon: <Clock className="h-5 w-5" />,
            label: "Age",
            value: `${product.age} years old`,
          },
        ]
      : []),
  ];

  return (
    <div className="mt-8 space-y-4">
      {infoItems.map((item, index) => (
        <div key={index} className="flex items-center gap-4">
          <div className="flex-shrink-0 text-muted-foreground">{item.icon}</div>
          <div>
            <p className="text-sm text-muted-foreground">{item.label}</p>
            <p className="font-semibold">{item.value}</p>
          </div>
        </div>
      ))}
    </div>
  );
};

const AddToCartSection = ({ product }: { product: Product }) => {
  const [quantity, setQuantity] = useState(1);
  const dispatch = useAppDispatch();
  const isAuthenticated = useAppSelector((state) => state.auth.isAuthenticated);
  const cartDetails = useAppSelector((state) => state.cart.cartDetails);
  const rowVersion = useAppSelector((state) => state.cart.rowVersion);
  const [syncCart] = useSyncCartMutation();
  const router = useRouter();
  const user = useAppSelector((state) => state.auth.user);
  const userId = user?.id;

  const handleAddToCart = async () => {
    if (!isAuthenticated) {
      toast.error("Please log in to add items to your cart.");
      router.push("/login");
      return;
    }

    if (typeof userId !== "number") {
      toast.error("Invalid user session. Please log in again.");
      router.push("/login");
      return;
    }

    dispatch(addItem({ product, quantity }));
    await syncCart({
      customerId: userId,
      items: [
        ...cartDetails.filter(
          (item: { productId: number }) => item.productId !== product.id
        ),
        {
          productId: product.id,
          quantity:
            (cartDetails.find(
              (item: { productId: number }) => item.productId === product.id
            )?.quantity || 0) + quantity,
        },
      ].map((item: { productId: number; quantity: number }) => ({
        productId: item.productId,
        quantity: item.quantity,
      })),
      rowVersion,
    });
  };

  return (
    <div className="mt-6">
      <Separator className="my-6" />
      <div className="flex items-center gap-4">
        <div className="flex items-center gap-2">
          <Button
            variant="outline"
            size="icon"
            onClick={() => setQuantity((q) => Math.max(1, q - 1))}
            disabled={quantity <= 1}
          >
            <Minus className="h-4 w-4" />
          </Button>
          <span className="text-lg font-bold w-12 text-center">{quantity}</span>
          <Button
            variant="outline"
            size="icon"
            onClick={() => setQuantity((q) => q + 1)}
            disabled={quantity >= product.stockQuantity}
          >
            <Plus className="h-4 w-4" />
          </Button>
        </div>
        <Button
          size="lg"
          className="flex-1"
          onClick={handleAddToCart}
          disabled={product.stockQuantity === 0}
        >
          {product.stockQuantity === 0 ? "Out of Stock" : "Add to Cart"}
        </Button>
      </div>
      {product.stockQuantity > 0 && (
        <p className="text-sm text-muted-foreground mt-2">
          {product.stockQuantity} items available
        </p>
      )}
    </div>
  );
};

function MoreOptionsDrawer({ currentProduct }: { currentProduct: Product }) {
  const [isOpen, setIsOpen] = useState(false);
  const { data: relatedProducts } = useGetProductsByBrandQuery(
    currentProduct.brandId,
    { skip: !currentProduct.brandId }
  );

  const filteredProducts = relatedProducts?.filter(
    (product: Product) => product.id !== currentProduct.id
  );

  return (
    <>
      <Button
        variant="outline"
        onClick={() => setIsOpen(true)}
        className="w-full"
      >
        More Options
        <ChevronRight className="ml-2 h-4 w-4" />
      </Button>

      <Sheet open={isOpen} onOpenChange={setIsOpen}>
        <SheetContent className="w-[400px] sm:w-[540px]">
          <SheetHeader>
            <SheetTitle>More from {currentProduct.brandName}</SheetTitle>
            <SheetClose />
          </SheetHeader>
          <div className="mt-6 space-y-4">
            {filteredProducts?.slice(0, 5).map((product: Product) => (
              <Link
                key={product.id}
                href={`/product/${product.slug}-${product.id}`}
                onClick={() => setIsOpen(false)}
                className="flex items-center gap-4 p-3 rounded-lg border hover:bg-gray-50 transition-colors"
              >
                <div className="relative w-16 h-16 rounded-md overflow-hidden">
                  <Image
                    src={product.imageUrl || "/product.png"}
                    alt={product.name}
                    fill
                    className="object-cover"
                  />
                </div>
                <div className="flex-1">
                  <h4 className="font-semibold text-sm">{product.name}</h4>
                  <p className="text-sm text-muted-foreground">
                    {product.volume}cl • {product.alcoholContent}% ABV
                  </p>
                  <p className="text-sm font-semibold text-green-600">
                    ${product.price}
                  </p>
                </div>
              </Link>
            ))}
          </div>
        </SheetContent>
      </Sheet>
    </>
  );
}

export default function ProductDetailClient({
  productId,
}: {
  productId: number;
}) {
  const { data: product, isLoading, error } = useGetProductByIdQuery(productId);

  if (isLoading) {
    return <ProductPageSkeleton />;
  }

  if (error || !product) {
    return (
      <div className="container mx-auto px-4 md:px-6 py-8">
        <div className="text-center">
          <h1 className="text-2xl font-bold mb-4">Product Not Found</h1>
          <p className="text-muted-foreground mb-6">
            The product you&apos;re looking for doesn&apos;t exist or has been
            removed.
          </p>
          <Link href="/">
            <Button>Back to Home</Button>
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 md:px-6 py-8">
      {/* Breadcrumb */}
      <nav className="mb-6">
        <ol className="flex items-center space-x-2 text-sm text-muted-foreground">
          <li>
            <Link href="/" className="hover:text-foreground">
              Home
            </Link>
          </li>
          <li>/</li>
          <li>
            <Link
              href={`/category/${product.categoryName
                .toLowerCase()
                .replace(/\s+/g, "-")}-${product.categoryId}`}
              className="hover:text-foreground"
            >
              {product.categoryName}
            </Link>
          </li>
          <li>/</li>
          <li className="text-foreground">{product.name}</li>
        </ol>
      </nav>

      <div className="grid md:grid-cols-12 gap-8 lg:gap-12">
        {/* Left Column: Info List */}
        <div className="hidden md:block md:col-span-3 lg:col-span-2">
          <ProductInfoList product={product} />
        </div>

        {/* Center Column: Image */}
        <div className="md:col-span-5 lg:col-span-6">
          <div className="relative aspect-square rounded-lg overflow-hidden bg-gray-100">
            <Image
              src={product.imageUrl || "/product.png"}
              alt={product.name}
              fill
              className="object-cover"
              priority
            />
          </div>
        </div>

        {/* Right Column: Details */}
        <div className="md:col-span-4 lg:col-span-4">
          <div className="sticky top-8">
            <div className="mb-4">
              <h1 className="text-3xl font-bold mb-2">{product.name}</h1>
              <p className="text-xl font-semibold text-green-600">
                ${product.price}
              </p>
            </div>

            <div className="mb-6">
              <p className="text-muted-foreground leading-relaxed">
                {product.description}
              </p>
            </div>

            <AddToCartSection product={product} />

            <div className="mt-6">
              <MoreOptionsDrawer currentProduct={product} />
            </div>

            {/* Mobile Info List */}
            <div className="md:hidden mt-8">
              <ProductInfoList product={product} />
            </div>
          </div>
        </div>
      </div>

      {/* Reviews Section */}
      <div className="mt-16">
        <ReviewSection productId={product.id} />
      </div>
    </div>
  );
}
