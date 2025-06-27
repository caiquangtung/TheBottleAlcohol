"use client";
import { useGetRecipesQuery } from "@/lib/services/recipeService";

export default function RecipeLibrary() {
  const { data: recipes, isLoading, error } = useGetRecipesQuery();

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading recipes</div>;

  return (
    <div>
      <h1 className="text-3xl font-bold mb-4">Recipe Library</h1>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        {recipes?.map((recipe) => (
          <div key={recipe.id} className="bg-white rounded shadow p-4">
            {recipe.imageUrl && (
              <img
                src={recipe.imageUrl}
                alt={recipe.name}
                className="w-full h-40 object-cover rounded mb-2"
              />
            )}
            <h2 className="text-xl font-semibold">{recipe.name}</h2>
            <p className="text-gray-600 line-clamp-2">{recipe.description}</p>
            {/* Thêm nút xem chi tiết nếu cần */}
          </div>
        ))}
      </div>
    </div>
  );
}
