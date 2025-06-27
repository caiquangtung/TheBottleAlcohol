import { useGetRecipeByIdQuery } from "@/lib/services/recipeService";
import { notFound } from "next/navigation";

interface RecipeDetailPageProps {
  params: { slugAndId: string };
}

function extractId(slugAndId: string): number | null {
  const parts = slugAndId.split("-");
  const idStr = parts[parts.length - 1];
  const id = Number(idStr);
  return isNaN(id) ? null : id;
}

export default function RecipeDetailPage({ params }: RecipeDetailPageProps) {
  const recipeId = extractId(params.slugAndId);
  const { data: recipe, isLoading, error } = useGetRecipeByIdQuery(recipeId!);

  if (!recipeId) return notFound();
  if (isLoading) return <div>Loading...</div>;
  if (error || !recipe) return notFound();

  return (
    <div className="max-w-3xl mx-auto py-8">
      <h1 className="text-3xl font-bold mb-4 text-center bg-green-700 text-white py-4 rounded">
        {recipe.name}
      </h1>
      {recipe.imageUrl && (
        <img
          src={recipe.imageUrl}
          alt={recipe.name}
          className="w-full h-80 object-cover rounded mb-6 mx-auto"
        />
      )}
      <div className="mb-4 text-lg text-gray-700 text-center">
        {recipe.description}
      </div>
      <div className="mb-6">
        <h2 className="text-xl font-semibold mb-2">Ingredients</h2>
        <ul className="list-disc list-inside">
          {recipe.ingredients?.map((ing) => (
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
