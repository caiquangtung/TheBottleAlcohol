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
  useGetAllCategoriesQuery,
  useCreateCategoryMutation,
  useUpdateCategoryMutation,
  useDeleteCategoryMutation,
} from "@/lib/services/categoryService";
import {
  Category,
  CategoryCreateDto,
  CategoryUpdateDto,
} from "@/lib/types/category";
import SearchInput from "@/components/admin/SearchInput";
import { Card, CardContent } from "@/components/ui/card";

export default function CategoriesPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [isUpdateDialogOpen, setIsUpdateDialogOpen] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState<Category | null>(
    null
  );
  const [createForm, setCreateForm] = useState<CategoryCreateDto>({
    name: "",
    description: "",
    slug: "",
    isActive: true,
  });
  const [updateForm, setUpdateForm] = useState<CategoryUpdateDto>({
    name: "",
    description: "",
    slug: "",
    isActive: true,
  });

  // RTK Query hooks
  const {
    data: categories = [],
    isLoading,
    error,
  } = useGetAllCategoriesQuery({ SearchTerm: searchTerm });
  const [createCategory, { isLoading: isCreating }] =
    useCreateCategoryMutation();
  const [updateCategory, { isLoading: isUpdating }] =
    useUpdateCategoryMutation();
  const [deleteCategory, { isLoading: isDeleting }] =
    useDeleteCategoryMutation();

  const handleCreate = async () => {
    try {
      await createCategory(createForm).unwrap();
      toast.success("Category created successfully");
      setIsCreateDialogOpen(false);
      setCreateForm({ name: "", description: "", slug: "", isActive: true });
    } catch (error) {
      toast.error("Failed to create category");
      console.error("Error creating category:", error);
    }
  };

  const handleUpdate = async () => {
    if (!selectedCategory) return;

    try {
      await updateCategory({
        id: selectedCategory.id,
        category: updateForm,
      }).unwrap();
      toast.success("Category updated successfully");
      setIsUpdateDialogOpen(false);
      setSelectedCategory(null);
      setUpdateForm({ name: "", description: "", slug: "", isActive: true });
    } catch (error) {
      toast.error("Failed to update category");
      console.error("Error updating category:", error);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteCategory(id).unwrap();
      toast.success("Category deleted successfully");
    } catch (error) {
      toast.error("Failed to delete category");
      console.error("Error deleting category:", error);
    }
  };

  const openUpdateDialog = (category: Category) => {
    setSelectedCategory(category);
    setUpdateForm({
      name: category.name,
      description: category.description,
      slug: category.slug,
      isActive: category.isActive,
    });
    setIsUpdateDialogOpen(true);
  };

  // Đã filter trên server, không cần filter client nữa
  const filteredCategories = Array.isArray(categories) ? categories : [];

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
        <div className="text-red-500">Failed to load categories</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Categories Management</h1>
        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Add Category
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Create New Category</DialogTitle>
              <DialogDescription>
                Add a new category to organize products.
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label htmlFor="name">Name</Label>
                <Input
                  id="name"
                  value={createForm.name}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, name: e.target.value })
                  }
                  placeholder="Category name"
                />
              </div>
              <div>
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
                  placeholder="Category description"
                />
              </div>
              <div>
                <Label htmlFor="slug">Slug</Label>
                <Input
                  id="slug"
                  value={createForm.slug}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, slug: e.target.value })
                  }
                  placeholder="category-slug"
                />
              </div>
              <div className="flex items-center space-x-2">
                <input
                  type="checkbox"
                  id="isActive"
                  checked={createForm.isActive}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, isActive: e.target.checked })
                  }
                  className="rounded"
                />
                <Label htmlFor="isActive">Active</Label>
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
                {isCreating ? (
                  <Loader2 className="h-4 w-4 animate-spin" />
                ) : (
                  "Create"
                )}
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>

      {/* Search */}
      <Card>
        <CardContent className="pt-6">
          <SearchInput
            value={searchTerm}
            onChange={setSearchTerm}
            placeholder="Search categories..."
          />
        </CardContent>
      </Card>

      <div className="border rounded-lg">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>ID</TableHead>
              <TableHead>Name</TableHead>
              <TableHead>Description</TableHead>
              <TableHead>Slug</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Created At</TableHead>
              <TableHead>Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredCategories.map((category) => (
              <TableRow key={category.id}>
                <TableCell className="font-mono text-sm">
                  {category.id}
                </TableCell>
                <TableCell className="font-medium">{category.name}</TableCell>
                <TableCell className="max-w-xs truncate">
                  {category.description}
                </TableCell>
                <TableCell className="font-mono text-sm">
                  {category.slug}
                </TableCell>
                <TableCell>
                  <Badge variant={category.isActive ? "default" : "secondary"}>
                    {category.isActive ? "Active" : "Inactive"}
                  </Badge>
                </TableCell>
                <TableCell className="text-sm text-muted-foreground">
                  {new Date(category.createdAt).toLocaleDateString()}
                </TableCell>
                <TableCell>
                  <div className="flex space-x-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => openUpdateDialog(category)}
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
                          <AlertDialogTitle>Delete Category</AlertDialogTitle>
                          <AlertDialogDescription>
                            Are you sure you want to delete "{category.name}"?
                            This action cannot be undone.
                          </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                          <AlertDialogCancel>Cancel</AlertDialogCancel>
                          <AlertDialogAction
                            onClick={() => handleDelete(category.id)}
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

      <Dialog open={isUpdateDialogOpen} onOpenChange={setIsUpdateDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Update Category</DialogTitle>
            <DialogDescription>Update category information.</DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="update-name">Name</Label>
              <Input
                id="update-name"
                value={updateForm.name}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, name: e.target.value })
                }
                placeholder="Category name"
              />
            </div>
            <div>
              <Label htmlFor="update-description">Description</Label>
              <Textarea
                id="update-description"
                value={updateForm.description}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, description: e.target.value })
                }
                placeholder="Category description"
              />
            </div>
            <div>
              <Label htmlFor="update-slug">Slug</Label>
              <Input
                id="update-slug"
                value={updateForm.slug}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, slug: e.target.value })
                }
                placeholder="category-slug"
              />
            </div>
            <div className="flex items-center space-x-2">
              <input
                type="checkbox"
                id="update-isActive"
                checked={updateForm.isActive}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, isActive: e.target.checked })
                }
                className="rounded"
              />
              <Label htmlFor="update-isActive">Active</Label>
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
              {isUpdating ? (
                <Loader2 className="h-4 w-4 animate-spin" />
              ) : (
                "Update"
              )}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
