import { notFound } from "next/navigation";
import { Metadata } from "next";
import ProductDetailClient from "./ProductDetailClient";
import { getProductById } from "@/lib/services/productService";

function extractId(slugAndId: string): number | null {
  const parts = slugAndId.split("-");
  const idStr = parts[parts.length - 1];
  const id = Number(idStr);
  return isNaN(id) ? null : id;
}

interface PageProps {
  params: Promise<{ slugAndId: string }>;
}

export async function generateMetadata({
  params,
}: PageProps): Promise<Metadata> {
  const { slugAndId } = await params;
  const productId = extractId(slugAndId);

  if (!productId) {
    return {
      title: "Product Not Found",
      description: "The requested product could not be found.",
    };
  }

  try {
    const product = await getProductById(productId);

    if (!product) {
      return {
        title: "Product Not Found",
        description: "The requested product could not be found.",
      };
    }

    return {
      title: product.metaTitle || `${product.name} - ${product.brandName}`,
      description:
        product.metaDescription ||
        product.description ||
        `Discover ${product.name} by ${product.brandName}. ${
          product.description ||
          "Premium quality alcohol with exceptional taste and craftsmanship."
        }`,
      keywords: `${product.name}, ${product.brandName}, ${product.categoryName}, alcohol, spirits, ${product.origin}, ${product.alcoholContent}% ABV`,
      openGraph: {
        title: product.metaTitle || `${product.name} - ${product.brandName}`,
        description:
          product.metaDescription ||
          product.description ||
          `Discover ${product.name} by ${product.brandName}. Premium quality alcohol with exceptional taste and craftsmanship.`,
        images: product.imageUrl
          ? [{ url: product.imageUrl, alt: product.name }]
          : [],
        type: "website",
      },
      twitter: {
        card: "summary_large_image",
        title: product.metaTitle || `${product.name} - ${product.brandName}`,
        description:
          product.metaDescription ||
          product.description ||
          `Discover ${product.name} by ${product.brandName}. Premium quality alcohol with exceptional taste and craftsmanship.`,
        images: product.imageUrl ? [product.imageUrl] : [],
      },
      alternates: {
        canonical: `/product/${slugAndId}`,
      },
    };
  } catch {
    return {
      title: "Product Not Found",
      description: "The requested product could not be found.",
    };
  }
}

export default async function ProductDetailPage({ params }: PageProps) {
  const { slugAndId } = await params;
  const productId = extractId(slugAndId);

  if (!productId) return notFound();

  return <ProductDetailClient productId={productId} />;
}
