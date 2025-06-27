"use client";
import React, { useState } from "react";
import { useParams } from "next/navigation";
import { useGetProductByIdQuery } from "@/lib/services/productService";
import { Product } from "@/lib/types/product";
import { Skeleton } from "@/components/ui/skeleton";
import Image from "next/image";
import { Button } from "@/components/ui/button";
import Link from "next/link";
import {
  Globe,
  Tag,
  MapPin,
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
import { ProductCardList } from "@/components/ProductCard";
import { useGetProductsByBrandQuery } from "@/lib/services/productService";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetClose,
} from "@/components/ui/sheet";

function extractId(slugAndId?: string) {
  if (!slugAndId) return NaN;
  const parts = slugAndId.split("-");
  return parseInt(parts[parts.length - 1], 10);
}

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

  const handleAddToCart = async () => {
    if (!isAuthenticated) {
      toast.error("Please log in to add items to your cart.");
      router.push("/login");
      return;
    }
    dispatch(addItem({ product, quantity }));
    await syncCart({
      items: [
        ...cartDetails.filter((item) => item.productId !== product.id),
        {
          productId: product.id,
          quantity:
            (cartDetails.find((item) => item.productId === product.id)
              ?.quantity || 0) + quantity,
        },
      ].map((item) => ({ productId: item.productId, quantity: item.quantity })),
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
          Add to Cart
        </Button>
      </div>
      <p className="text-sm text-green-600 mt-2 font-semibold">
        {product.stockQuantity > 0 ? "In Stock" : "Out of Stock"}
      </p>
    </div>
  );
};

function MoreOptionsDrawer({ currentProduct }: { currentProduct: Product }) {
  const [open, setOpen] = useState(false);
  const { data: products = [], isLoading } = useGetProductsByBrandQuery(
    currentProduct.brandId,
    { skip: !currentProduct.brandId || !open }
  );
  // Lọc bỏ sản phẩm hiện tại
  const filtered = products.filter((p) => p.id !== currentProduct.id);
  return (
    <>
      <Button
        variant="ghost"
        className="w-full justify-between px-0 font-medium text-base"
        onClick={() => setOpen(true)}
      >
        More options
        <ChevronRight className="h-5 w-5" />
      </Button>
      <Sheet open={open} onOpenChange={setOpen}>
        <SheetContent className="w-full max-w-lg sm:max-w-xl flex flex-col">
          <SheetHeader>
            <SheetTitle>RELATED:</SheetTitle>
            <SheetClose asChild>
              <Button variant="ghost" className="absolute right-2 top-2">
                Close
              </Button>
            </SheetClose>
          </SheetHeader>
          <div className="flex-1 overflow-y-auto mt-4">
            {isLoading ? (
              <div className="text-center py-8 text-muted-foreground">
                Loading...
              </div>
            ) : filtered.length === 0 ? (
              <div className="text-center py-8 text-muted-foreground">
                No other products from this brand.
              </div>
            ) : (
              <ul className="divide-y divide-muted-foreground/10">
                {filtered.map((p) => (
                  <li key={p.id}>
                    <Link
                      href={`/product/${p.slug}-${p.id}`}
                      className="flex items-center gap-6 px-4 py-5 hover:bg-accent rounded-lg transition"
                    >
                      <div className="relative w-20 h-20 flex-shrink-0">
                        <Image
                          src={p.imageUrl || "/product.png"}
                          alt={p.name}
                          fill
                          className="object-contain rounded-lg"
                        />
                      </div>
                      <div className="flex-1 min-w-0">
                        <div className="font-semibold text-lg truncate">
                          {p.name}
                        </div>
                        <div className="text-base text-muted-foreground font-bold mt-1">
                          {new Intl.NumberFormat("en-GB", {
                            style: "currency",
                            currency: "GBP",
                          }).format(p.price)}
                        </div>
                      </div>
                    </Link>
                  </li>
                ))}
              </ul>
            )}
          </div>
        </SheetContent>
      </Sheet>
    </>
  );
}

export default function ProductPage() {
  const params = useParams();
  const slugAndId = params.slugAndId as string;
  const id = extractId(slugAndId);

  const {
    data: product,
    isLoading,
    isError,
    refetch,
  } = useGetProductByIdQuery(id, {
    skip: isNaN(id),
  });

  if (isLoading) {
    return <ProductPageSkeleton />;
  }

  if (isError) {
    return (
      <div className="container mx-auto text-center py-16">
        <h2 className="text-2xl font-bold mb-4">Failed to load product</h2>
        <p className="text-muted-foreground mb-4">
          There was an error fetching the product details.
        </p>
        <Button onClick={() => refetch()}>Try Again</Button>
      </div>
    );
  }

  if (!product) {
    return (
      <div className="container mx-auto text-center py-16">
        <h2 className="text-2xl font-bold">Product not found</h2>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 md:px-6 py-8">
      {/* Breadcrumb */}
      <div className="mb-6 text-sm text-muted-foreground">
        <Link href="/" className="hover:text-primary">
          Home
        </Link>
        {" / "}
        <Link
          href={`/category/${product.categoryName.toLowerCase()}-${
            product.categoryId
          }`}
          className="hover:text-primary"
        >
          {product.categoryName}
        </Link>
        {" / "}
        <span>{product.name}</span>
      </div>

      <div className="grid md:grid-cols-12 gap-8 lg:gap-12">
        {/* Left Column: Info List */}
        <div className="hidden md:block md:col-span-3 lg:col-span-2">
          <ProductInfoList product={product} />
        </div>

        {/* Center Column: Image */}
        <div className="md:col-span-5 lg:col-span-6">
          <div className="aspect-square relative  rounded-lg overflow-hidden">
            <Image
              src={product.imageUrl || "/product.png"}
              alt={product.name}
              fill
              className="object-contain"
            />
          </div>
        </div>

        {/* Right Column: Details */}
        <div className="md:col-span-4 lg:col-span-4">
          <h1 className="text-3xl font-bold tracking-tight">{product.name}</h1>
          <p className="text-2xl mt-2 mb-4 font-semibold">
            {new Intl.NumberFormat("en-GB", {
              style: "currency",
              currency: "GBP",
            }).format(product.price)}
          </p>
          <div className="prose max-w-none text-muted-foreground">
            <p>{product.description}</p>
          </div>
          <AddToCartSection product={product} />
          <MoreOptionsDrawer currentProduct={product} />
        </div>
      </div>
      {/* More product details will go here */}
      {product && <ReviewSection productId={product.id} />}
      {/* More Options: Drawer */}
    </div>
  );
}
