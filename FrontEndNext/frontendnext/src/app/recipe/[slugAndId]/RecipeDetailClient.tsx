"use client";
import { useGetRecipeByIdQuery } from "@/lib/services/recipeService";
import { notFound } from "next/navigation";
import Image from "next/image";
import { RecipeIngredient } from "@/lib/types/recipe";

interface RecipeDetailClientProps {
  recipeId: number;
}

export default function RecipeDetailClient({
  recipeId,
}: RecipeDetailClientProps) {
  const { data: recipe, isLoading, error } = useGetRecipeByIdQuery(recipeId);

  if (isLoading) return <div>Loading...</div>;
  if (error || !recipe) return notFound();

  return (
    <div className="max-w-3xl mx-auto py-8">
      <h1 className="text-3xl font-bold mb-4 text-center bg-green-700 text-white py-4 rounded">
        {recipe.name}
      </h1>
      {recipe.imageUrl && (
        <Image
          src={recipe.imageUrl}
          alt={recipe.name}
          width={800}
          height={320}
          className="w-full h-80 object-cover rounded mb-6 mx-auto"
        />
      )}
      <div className="mb-4 text-lg text-gray-700 text-center">
        {recipe.description}
      </div>
      <div className="mb-6">
        <h2 className="text-xl font-semibold mb-2">Ingredients</h2>
        <ul className="list-disc list-inside">
          {recipe.ingredients?.map((ing: RecipeIngredient) => (
            <li key={ing.id}>
              {ing.quantity} {ing.unit} {ing.name}
              {ing.notes ? ` (${ing.notes})` : ""}
            </li>
          ))}
        </ul>
      </div>
      <div>
        <h2 className="text-xl font-semibold mb-2">Instructions</h2>
        <div className="whitespace-pre-line text-gray-800">
          {recipe.instructions}
        </div>
      </div>
    </div>
  );
}
