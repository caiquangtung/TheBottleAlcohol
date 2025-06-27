import { notFound } from "next/navigation";
import RecipeDetailClient from "./RecipeDetailClient";

function extractId(slugAndId: string): number | null {
  const parts = slugAndId.split("-");
  const idStr = parts[parts.length - 1];
  const id = Number(idStr);
  return isNaN(id) ? null : id;
}

interface PageProps {
  params: Promise<{ slugAndId: string }>;
}

export default async function RecipeDetailPage({ params }: PageProps) {
  const { slugAndId } = await params;
  const recipeId = extractId(slugAndId);

  if (!recipeId) return notFound();

  return <RecipeDetailClient recipeId={recipeId} />;
}
