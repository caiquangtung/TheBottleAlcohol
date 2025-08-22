"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Textarea } from "@/components/ui/textarea";
import { Switch } from "@/components/ui/switch";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
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
import { toast } from "sonner";
import { Plus, Edit, Trash2, Search } from "lucide-react";
import {
  useGetProductsQuery,
  useCreateProductMutation,
  useUpdateProductMutation,
  useDeleteProductMutation,
} from "@/lib/services/productService";
import { useGetAllCategoriesQuery } from "@/lib/services/categoryService";
import { useGetAllBrandsQuery } from "@/lib/services/brandService";
import { generateSlug } from "@/lib/utils/utils";
import type {
  Product,
  ProductCreate,
  ProductUpdate,
} from "@/lib/types/product";
import SearchInput from "@/components/admin/SearchInput";
import { CloudinaryUploadButton } from "@/components/CloudinaryUploadButton";

export default function AdminProductsPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);

  // Queries
  const {
    data: productsData,
    isLoading,
    error,
    refetch,
  } = useGetProductsQuery({ SearchTerm: searchTerm });
  const { data: categoriesData } = useGetAllCategoriesQuery();
  const { data: brandsData } = useGetAllBrandsQuery();

  // Mutations
  const [createProduct, { isLoading: isCreating }] = useCreateProductMutation();
  const [updateProduct, { isLoading: isUpdating }] = useUpdateProductMutation();
  const [deleteProduct, { isLoading: isDeleting }] = useDeleteProductMutation();

  const products = productsData?.items || [];
  const [deletingId, setDeletingId] = useState<number | null>(null);

  const handleCreateProduct = async (formData: ProductCreate) => {
    try {
      await createProduct(formData).unwrap();
      toast.success("Product created successfully");
      setIsCreateModalOpen(false);
      refetch();
    } catch (error: any) {
      toast.error(error?.data?.message || "Failed to create product");
    }
  };

  const handleUpdateProduct = async (formData: ProductUpdate) => {
    if (!editingProduct) return;
    try {
      await updateProduct({ id: editingProduct.id, data: formData }).unwrap();
      toast.success("Product updated successfully");
      setIsEditModalOpen(false);
      setEditingProduct(null);
      refetch();
    } catch (error: any) {
      toast.error(error?.data?.message || "Failed to update product");
    }
  };

  const handleDeleteProduct = async (productId: number) => {
    try {
      await deleteProduct(productId).unwrap();
      toast.success("Product deleted successfully");
      setDeletingId(null);
      refetch();
    } catch (error: any) {
      toast.error(error?.data?.message || "Failed to delete product");
    }
  };

  const openEditModal = (product: Product) => {
    setEditingProduct(product);
    setIsEditModalOpen(true);
  };

  const formatPrice = (price: number) => {
    return new Intl.NumberFormat("vi-VN", {
      style: "currency",
      currency: "VND",
    }).format(price);
  };

  if (error) {
    return (
      <div className="p-6">
        <Card>
          <CardContent className="pt-6">
            <div className="text-center text-red-600">
              Failed to load products
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Products Management</h1>
        <Dialog open={isCreateModalOpen} onOpenChange={setIsCreateModalOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="w-4 h-4 mr-2" />
              Add Product
            </Button>
          </DialogTrigger>
          <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>Add New Product</DialogTitle>
            </DialogHeader>
            <ProductForm
              onSubmit={handleCreateProduct}
              isLoading={isCreating}
              categories={categoriesData || []}
              brands={brandsData || []}
            />
          </DialogContent>
        </Dialog>
      </div>

      {/* Search */}
      <Card>
        <CardContent className="pt-6">
          <SearchInput
            value={searchTerm}
            onChange={setSearchTerm}
            placeholder="Search products..."
          />
        </CardContent>
      </Card>

      {/* Products Table */}
      <Card>
        <CardHeader>
          <CardTitle>Products ({products.length})</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-8">Loading...</div>
          ) : products.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No products found
            </div>
          ) : (
            <div className="border rounded-lg">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>ID</TableHead>
                    <TableHead>Product Name</TableHead>
                    <TableHead>Category</TableHead>
                    <TableHead>Brand</TableHead>
                    <TableHead>Price</TableHead>
                    <TableHead>Stock</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {products.map((product: Product) => (
                    <TableRow key={product.id}>
                      <TableCell className="font-mono text-sm">
                        #{product.id}
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{product.name}</div>
                          <div className="text-sm text-muted-foreground">
                            {product.volume}L - {product.alcoholContent}%
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>{product.categoryName}</TableCell>
                      <TableCell>{product.brandName}</TableCell>
                      <TableCell>{formatPrice(product.price)}</TableCell>
                      <TableCell>{product.stockQuantity}</TableCell>
                      <TableCell>
                        <Badge
                          variant={product.status ? "default" : "secondary"}
                        >
                          {product.status ? "Active" : "Inactive"}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="flex space-x-2">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => openEditModal(product)}
                          >
                            <Edit className="w-4 h-4" />
                          </Button>
                          <AlertDialog
                            open={deletingId === product.id}
                            onOpenChange={(open) =>
                              setDeletingId(open ? product.id : null)
                            }
                          >
                            <AlertDialogTrigger asChild>
                              <Button variant="outline" size="sm">
                                <Trash2 className="w-4 h-4" />
                              </Button>
                            </AlertDialogTrigger>
                            <AlertDialogContent>
                              <AlertDialogHeader>
                                <AlertDialogTitle>
                                  Delete Confirmation
                                </AlertDialogTitle>
                                <AlertDialogDescription>
                                  Are you sure you want to delete "
                                  {product.name}"? This action cannot be undone.
                                </AlertDialogDescription>
                              </AlertDialogHeader>
                              <AlertDialogFooter>
                                <AlertDialogCancel>Cancel</AlertDialogCancel>
                                <AlertDialogAction
                                  onClick={() =>
                                    handleDeleteProduct(product.id)
                                  }
                                  disabled={isDeleting}
                                  className="bg-red-600 hover:bg-red-700"
                                >
                                  {isDeleting ? "Deleting..." : "Delete"}
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
          )}
        </CardContent>
      </Card>

      {/* Create Modal */}
      <Dialog open={isCreateModalOpen} onOpenChange={setIsCreateModalOpen}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Add New Product</DialogTitle>
          </DialogHeader>
          <ProductForm
            onSubmit={handleCreateProduct}
            isLoading={isCreating}
            categories={categoriesData || []}
            brands={brandsData || []}
          />
        </DialogContent>
      </Dialog>

      {/* Edit Modal */}
      <Dialog open={isEditModalOpen} onOpenChange={setIsEditModalOpen}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Edit Product</DialogTitle>
          </DialogHeader>
          {editingProduct && (
            <ProductForm
              onSubmit={handleUpdateProduct}
              isLoading={isUpdating}
              initialData={editingProduct}
              categories={categoriesData || []}
              brands={brandsData || []}
            />
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}

interface ProductFormProps {
  onSubmit: (data: ProductCreate | ProductUpdate) => void;
  isLoading: boolean;
  initialData?: Product;
  categories: any[];
  brands: any[];
}

function ProductForm({
  onSubmit,
  isLoading,
  initialData,
  categories,
  brands,
}: ProductFormProps) {
  const [formData, setFormData] = useState<ProductCreate>({
    name: initialData?.name || "",
    description: initialData?.description || "",
    slug: initialData?.slug || "",
    origin: initialData?.origin || "",
    volume: initialData?.volume || 0,
    alcoholContent: initialData?.alcoholContent || 0,
    price: initialData?.price || 0,
    stockQuantity: initialData?.stockQuantity || 0,
    status: initialData?.status ?? true,
    imageUrl: initialData?.imageUrl || "",
    metaTitle: initialData?.metaTitle || "",
    metaDescription: initialData?.metaDescription || "",
    categoryId: initialData?.categoryId || 0,
    brandId: initialData?.brandId || 0,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  const handleInputChange = (field: keyof ProductCreate, value: any) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  // Xử lý khi thay đổi tên sản phẩm - tự động tạo slug
  const handleNameChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const name = e.target.value;
    const slug = generateSlug(name);

    setFormData((prev) => ({
      ...prev,
      name: name,
      slug: slug,
      // Tự động tạo meta title nếu chưa có
      metaTitle: prev.metaTitle || name,
    }));
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="name">Product Name *</Label>
          <Input
            id="name"
            value={formData.name}
            onChange={handleNameChange}
            required
            placeholder="Enter product name..."
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="slug">Slug</Label>
          <Input
            id="slug"
            value={formData.slug}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("slug", e.target.value)
            }
            placeholder="auto-generated-slug"
            className="bg-gray-50"
          />
          <p className="text-xs text-muted-foreground">
            Slug will be generated automatically from the product name
          </p>
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">Description</Label>
        <Textarea
          id="description"
          value={formData.description}
          onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) =>
            handleInputChange("description", e.target.value)
          }
          rows={3}
        />
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="origin">Origin</Label>
          <Input
            id="origin"
            value={formData.origin}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("origin", e.target.value)
            }
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="volume">Volume (L) *</Label>
          <Input
            id="volume"
            type="number"
            step="0.01"
            min="0.01"
            max="10"
            value={formData.volume}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("volume", parseFloat(e.target.value) || 0)
            }
            required
          />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="alcoholContent">Alcohol content (%) *</Label>
          <Input
            id="alcoholContent"
            type="number"
            step="0.1"
            min="0"
            max="100"
            value={formData.alcoholContent}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange(
                "alcoholContent",
                parseFloat(e.target.value) || 0
              )
            }
            required
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="price">Price (VND) *</Label>
          <Input
            id="price"
            type="number"
            step="1000"
            min="0"
            value={formData.price}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("price", parseFloat(e.target.value) || 0)
            }
            required
          />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="stockQuantity">Stock quantity *</Label>
          <Input
            id="stockQuantity"
            type="number"
            min="0"
            value={formData.stockQuantity}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("stockQuantity", parseInt(e.target.value) || 0)
            }
            required
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="categoryId">Category *</Label>
          <Select
            value={formData.categoryId ? formData.categoryId.toString() : ""}
            onValueChange={(value) =>
              handleInputChange("categoryId", parseInt(value))
            }
          >
            <SelectTrigger>
              <SelectValue placeholder="Select category" />
            </SelectTrigger>
            <SelectContent>
              {categories.map((category) => (
                <SelectItem key={category.id} value={category.id.toString()}>
                  {category.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="brandId">Brand *</Label>
          <Select
            value={formData.brandId ? formData.brandId.toString() : ""}
            onValueChange={(value) =>
              handleInputChange("brandId", parseInt(value))
            }
          >
            <SelectTrigger>
              <SelectValue placeholder="Select brand" />
            </SelectTrigger>
            <SelectContent>
              {brands.map((brand) => (
                <SelectItem key={brand.id} value={brand.id.toString()}>
                  {brand.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-2">
          <Label htmlFor="imageUrl">Product Image</Label>
          <CloudinaryUploadButton
            value={formData.imageUrl}
            onChange={(url) => handleInputChange("imageUrl", url)}
            label={formData.imageUrl ? "Change Image" : "Upload Image"}
          />
          <Input
            id="imageUrl"
            placeholder="Or paste an image URL"
            value={formData.imageUrl}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("imageUrl", e.target.value)
            }
          />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="metaTitle">Meta Title</Label>
          <Input
            id="metaTitle"
            value={formData.metaTitle}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("metaTitle", e.target.value)
            }
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="metaDescription">Meta Description</Label>
          <Input
            id="metaDescription"
            value={formData.metaDescription}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("metaDescription", e.target.value)
            }
          />
        </div>
      </div>

      <div className="flex items-center space-x-2">
        <Switch
          id="status"
          checked={formData.status}
          onCheckedChange={(checked: boolean) =>
            handleInputChange("status", checked)
          }
        />
        <Label htmlFor="status">Active</Label>
      </div>

      <div className="flex justify-end space-x-2 pt-4">
        <Button type="submit" disabled={isLoading}>
          {isLoading ? "Saving..." : initialData ? "Update" : "Create Product"}
        </Button>
      </div>
    </form>
  );
}
