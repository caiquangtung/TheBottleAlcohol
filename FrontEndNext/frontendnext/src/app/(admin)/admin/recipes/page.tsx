"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { Plus, Search, Edit, Trash2, Loader2 } from "lucide-react";
import { toast } from "sonner";
import {
  useGetRecipesQuery,
  useCreateRecipeMutation,
  useUpdateRecipeMutation,
  useDeleteRecipeMutation,
} from "@/lib/services/recipeService";
import { Recipe, RecipeCreateDto, RecipeUpdateDto } from "@/lib/types/recipe";

export default function RecipesPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [isUpdateDialogOpen, setIsUpdateDialogOpen] = useState(false);
  const [selectedRecipe, setSelectedRecipe] = useState<Recipe | null>(null);
  const [createForm, setCreateForm] = useState<RecipeCreateDto>({
    name: "",
    description: "",
    instructions: "",
    prepTime: 0,
    cookTime: 0,
    servings: 1,
    difficulty: "Easy",
    imageUrl: "",
    isFeatured: false,
    categoryIds: [],
    ingredients: [],
  });
  const [updateForm, setUpdateForm] = useState<RecipeUpdateDto>({
    name: "",
    description: "",
    instructions: "",
    prepTime: 0,
    cookTime: 0,
    servings: 1,
    difficulty: "Easy",
    imageUrl: "",
    isFeatured: false,
    categoryIds: [],
    ingredients: [],
  });

  // RTK Query hooks
  const { data: recipes = [], isLoading, error } = useGetRecipesQuery();
  const [createRecipe, { isLoading: isCreating }] = useCreateRecipeMutation();
  const [updateRecipe, { isLoading: isUpdating }] = useUpdateRecipeMutation();
  const [deleteRecipe, { isLoading: isDeleting }] = useDeleteRecipeMutation();

  const handleCreate = async () => {
    try {
      await createRecipe(createForm).unwrap();
      toast.success("Recipe created successfully");
      setIsCreateDialogOpen(false);
      setCreateForm({
        name: "",
        description: "",
        instructions: "",
        prepTime: 0,
        cookTime: 0,
        servings: 1,
        difficulty: "Easy",
        imageUrl: "",
        isFeatured: false,
        categoryIds: [],
        ingredients: [],
      });
    } catch (error) {
      toast.error("Failed to create recipe");
      console.error("Error creating recipe:", error);
    }
  };

  const handleUpdate = async () => {
    if (!selectedRecipe) return;

    try {
      await updateRecipe({
        id: selectedRecipe.id,
        recipe: updateForm,
      }).unwrap();
      toast.success("Recipe updated successfully");
      setIsUpdateDialogOpen(false);
      setSelectedRecipe(null);
      setUpdateForm({
        name: "",
        description: "",
        instructions: "",
        prepTime: 0,
        cookTime: 0,
        servings: 1,
        difficulty: "Easy",
        imageUrl: "",
        isFeatured: false,
        categoryIds: [],
        ingredients: [],
      });
    } catch (error) {
      toast.error("Failed to update recipe");
      console.error("Error updating recipe:", error);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteRecipe(id).unwrap();
      toast.success("Recipe deleted successfully");
    } catch (error) {
      toast.error("Failed to delete recipe");
      console.error("Error deleting recipe:", error);
    }
  };

  const openUpdateDialog = (recipe: Recipe) => {
    setSelectedRecipe(recipe);
    setUpdateForm({
      name: recipe.name,
      description: recipe.description,
      instructions: recipe.instructions,
      prepTime: recipe.prepTime,
      cookTime: recipe.cookTime,
      servings: recipe.servings,
      difficulty: recipe.difficulty,
      imageUrl: recipe.imageUrl,
      isFeatured: recipe.isFeatured,
      categoryIds: [],
      ingredients: [],
    });
    setIsUpdateDialogOpen(true);
  };

  const getDifficultyLabel = (difficulty: string) => {
    switch (difficulty) {
      case "Easy":
        return <Badge variant="default">Easy</Badge>;
      case "Medium":
        return <Badge variant="secondary">Medium</Badge>;
      case "Hard":
        return <Badge variant="destructive">Hard</Badge>;
      default:
        return <Badge variant="outline">Unknown</Badge>;
    }
  };

  const getTotalTime = (prepTime: number, cookTime: number) => {
    return prepTime + cookTime;
  };

  const filteredRecipes = recipes.filter(
    (recipe) =>
      recipe.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      recipe.description.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (isLoading) {
    return (
      <div className="flex items-center justify-center h-64">
        <Loader2 className="h-8 w-8 animate-spin" />
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-red-500">Failed to load recipes</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Recipes Management</h1>
        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Add Recipe
            </Button>
          </DialogTrigger>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Create New Recipe</DialogTitle>
              <DialogDescription>
                Add a new recipe to the system.
              </DialogDescription>
            </DialogHeader>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="name">Name</Label>
                <Input
                  id="name"
                  value={createForm.name}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, name: e.target.value })
                  }
                  placeholder="Recipe name"
                />
              </div>
              <div>
                <Label htmlFor="imageUrl">Image URL</Label>
                <Input
                  id="imageUrl"
                  value={createForm.imageUrl}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, imageUrl: e.target.value })
                  }
                  placeholder="https://example.com/image.jpg"
                />
              </div>
              <div className="col-span-2">
                <Label htmlFor="description">Description</Label>
                <Textarea
                  id="description"
                  value={createForm.description}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      description: e.target.value,
                    })
                  }
                  placeholder="Recipe description"
                />
              </div>
              <div className="col-span-2">
                <Label htmlFor="instructions">Instructions</Label>
                <Textarea
                  id="instructions"
                  value={createForm.instructions}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      instructions: e.target.value,
                    })
                  }
                  placeholder="Step-by-step instructions"
                  rows={4}
                />
              </div>
              <div>
                <Label htmlFor="prepTime">Preparation Time (minutes)</Label>
                <Input
                  id="prepTime"
                  type="number"
                  value={createForm.prepTime}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      prepTime: parseInt(e.target.value),
                    })
                  }
                  placeholder="15"
                />
              </div>
              <div>
                <Label htmlFor="cookTime">Cooking Time (minutes)</Label>
                <Input
                  id="cookTime"
                  type="number"
                  value={createForm.cookTime}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      cookTime: parseInt(e.target.value),
                    })
                  }
                  placeholder="30"
                />
              </div>
              <div>
                <Label htmlFor="servings">Servings</Label>
                <Input
                  id="servings"
                  type="number"
                  value={createForm.servings}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      servings: parseInt(e.target.value),
                    })
                  }
                  placeholder="4"
                />
              </div>
              <div>
                <Label htmlFor="difficulty">Difficulty</Label>
                <select
                  id="difficulty"
                  value={createForm.difficulty}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      difficulty: e.target.value,
                    })
                  }
                  className="w-full p-2 border rounded-md"
                >
                  <option value="Easy">Easy</option>
                  <option value="Medium">Medium</option>
                  <option value="Hard">Hard</option>
                </select>
              </div>
            </div>
            <DialogFooter>
              <Button
                variant="outline"
                onClick={() => setIsCreateDialogOpen(false)}
              >
                Cancel
              </Button>
              <Button onClick={handleCreate} disabled={isCreating}>
                {isCreating && (
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                )}
                Create
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      <div className="flex items-center space-x-2">
        <div className="relative flex-1 max-w-sm">
          <Search className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Search recipes..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-8"
          />
        </div>
      </div>

      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Name</TableHead>
              <TableHead>Description</TableHead>
              <TableHead>Total Time</TableHead>
              <TableHead>Servings</TableHead>
              <TableHead>Difficulty</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredRecipes.map((recipe) => (
              <TableRow key={recipe.id}>
                <TableCell className="font-medium">{recipe.name}</TableCell>
                <TableCell className="max-w-xs truncate">
                  {recipe.description}
                </TableCell>
                <TableCell>
                  {getTotalTime(recipe.prepTime, recipe.cookTime)} min
                </TableCell>
                <TableCell>{recipe.servings}</TableCell>
                <TableCell>{getDifficultyLabel(recipe.difficulty)}</TableCell>
                <TableCell>
                  <Badge variant={recipe.isFeatured ? "default" : "secondary"}>
                    {recipe.isFeatured ? "Featured" : "Regular"}
                  </Badge>
                </TableCell>
                <TableCell>
                  <div className="flex items-center space-x-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => openUpdateDialog(recipe)}
                    >
                      <Edit className="h-4 w-4" />
                    </Button>
                    <AlertDialog>
                      <AlertDialogTrigger asChild>
                        <Button variant="outline" size="sm">
                          <Trash2 className="h-4 w-4" />
                        </Button>
                      </AlertDialogTrigger>
                      <AlertDialogContent>
                        <AlertDialogHeader>
                          <AlertDialogTitle>Delete Recipe</AlertDialogTitle>
                          <AlertDialogDescription>
                            Are you sure you want to delete "{recipe.name}"?
                            This action cannot be undone.
                          </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                          <AlertDialogCancel>Cancel</AlertDialogCancel>
                          <AlertDialogAction
                            onClick={() => handleDelete(recipe.id)}
                            className="bg-red-600 hover:bg-red-700"
                          >
                            Delete
                          </AlertDialogAction>
                        </AlertDialogFooter>
                      </AlertDialogContent>
                    </AlertDialog>
                  </div>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {/* Update Dialog */}
      <Dialog open={isUpdateDialogOpen} onOpenChange={setIsUpdateDialogOpen}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Update Recipe</DialogTitle>
            <DialogDescription>
              Update the recipe information.
            </DialogDescription>
          </DialogHeader>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label htmlFor="update-name">Name</Label>
              <Input
                id="update-name"
                value={updateForm.name}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, name: e.target.value })
                }
                placeholder="Recipe name"
              />
            </div>
            <div>
              <Label htmlFor="update-imageUrl">Image URL</Label>
              <Input
                id="update-imageUrl"
                value={updateForm.imageUrl}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, imageUrl: e.target.value })
                }
                placeholder="https://example.com/image.jpg"
              />
            </div>
            <div className="col-span-2">
              <Label htmlFor="update-description">Description</Label>
              <Textarea
                id="update-description"
                value={updateForm.description}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    description: e.target.value,
                  })
                }
                placeholder="Recipe description"
              />
            </div>
            <div className="col-span-2">
              <Label htmlFor="update-instructions">Instructions</Label>
              <Textarea
                id="update-instructions"
                value={updateForm.instructions}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    instructions: e.target.value,
                  })
                }
                placeholder="Step-by-step instructions"
                rows={4}
              />
            </div>
            <div>
              <Label htmlFor="update-prepTime">
                Preparation Time (minutes)
              </Label>
              <Input
                id="update-prepTime"
                type="number"
                value={updateForm.prepTime}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    prepTime: parseInt(e.target.value),
                  })
                }
                placeholder="15"
              />
            </div>
            <div>
              <Label htmlFor="update-cookTime">Cooking Time (minutes)</Label>
              <Input
                id="update-cookTime"
                type="number"
                value={updateForm.cookTime}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    cookTime: parseInt(e.target.value),
                  })
                }
                placeholder="30"
              />
            </div>
            <div>
              <Label htmlFor="update-servings">Servings</Label>
              <Input
                id="update-servings"
                type="number"
                value={updateForm.servings}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    servings: parseInt(e.target.value),
                  })
                }
                placeholder="4"
              />
            </div>
            <div>
              <Label htmlFor="update-difficulty">Difficulty</Label>
              <select
                id="update-difficulty"
                value={updateForm.difficulty}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    difficulty: e.target.value,
                  })
                }
                className="w-full p-2 border rounded-md"
              >
                <option value="Easy">Easy</option>
                <option value="Medium">Medium</option>
                <option value="Hard">Hard</option>
              </select>
            </div>
          </div>
          <DialogFooter>
            <Button
              variant="outline"
              onClick={() => setIsUpdateDialogOpen(false)}
            >
              Cancel
            </Button>
            <Button onClick={handleUpdate} disabled={isUpdating}>
              {isUpdating && <Loader2 className="h-4 w-4 mr-2 animate-spin" />}
              Update
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
