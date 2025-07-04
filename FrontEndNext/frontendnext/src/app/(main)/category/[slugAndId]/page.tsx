import { notFound } from "next/navigation";
import { Metadata } from "next";
import CategoryDetailClient from "./CategoryDetailClient";
import { getCategoryById } from "@/lib/services/categoryService";

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
  const categoryId = extractId(slugAndId);

  if (!categoryId) {
    return {
      title: "Category Not Found",
      description: "The requested category could not be found.",
    };
  }

  try {
    const category = await getCategoryById(categoryId);

    if (!category) {
      return {
        title: "Category Not Found",
        description: "The requested category could not be found.",
      };
    }

    return {
      title: category.metaTitle || `${category.name} - Alcohol Store`,
      description:
        category.metaDescription ||
        category.description ||
        `Browse our collection of ${category.name.toLowerCase()} products. Premium quality alcohol with exceptional taste and craftsmanship.`,
      keywords: `${category.name}, alcohol, spirits, ${
        category.description
          ? category.description.split(" ").slice(0, 10).join(", ")
          : "premium alcohol"
      }`,
      openGraph: {
        title: category.metaTitle || `${category.name} - Alcohol Store`,
        description:
          category.metaDescription ||
          category.description ||
          `Browse our collection of ${category.name.toLowerCase()} products. Premium quality alcohol with exceptional taste and craftsmanship.`,
        images: category.imageUrl
          ? [{ url: category.imageUrl, alt: category.name }]
          : [],
        type: "website",
      },
      twitter: {
        card: "summary_large_image",
        title: category.metaTitle || `${category.name} - Alcohol Store`,
        description:
          category.metaDescription ||
          category.description ||
          `Browse our collection of ${category.name.toLowerCase()} products. Premium quality alcohol with exceptional taste and craftsmanship.`,
        images: category.imageUrl ? [category.imageUrl] : [],
      },
      alternates: {
        canonical: `/category/${slugAndId}`,
      },
    };
  } catch {
    return {
      title: "Category Not Found",
      description: "The requested category could not be found.",
    };
  }
}

export default async function CategoryDetailPage({ params }: PageProps) {
  const { slugAndId } = await params;
  const categoryId = extractId(slugAndId);

  if (!categoryId) return notFound();

  return <CategoryDetailClient categoryId={categoryId} />;
}
