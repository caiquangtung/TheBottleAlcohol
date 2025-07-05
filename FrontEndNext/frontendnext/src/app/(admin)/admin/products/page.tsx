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
  } = useGetProductsQuery({ search: searchTerm });
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
      toast.success("Sản phẩm đã được tạo thành công!");
      setIsCreateModalOpen(false);
      refetch();
    } catch (error: any) {
      toast.error(error?.data?.message || "Có lỗi xảy ra khi tạo sản phẩm");
    }
  };

  const handleUpdateProduct = async (formData: ProductUpdate) => {
    if (!editingProduct) return;
    try {
      await updateProduct({ id: editingProduct.id, data: formData }).unwrap();
      toast.success("Sản phẩm đã được cập nhật thành công!");
      setIsEditModalOpen(false);
      setEditingProduct(null);
      refetch();
    } catch (error: any) {
      toast.error(
        error?.data?.message || "Có lỗi xảy ra khi cập nhật sản phẩm"
      );
    }
  };

  const handleDeleteProduct = async (productId: number) => {
    try {
      await deleteProduct(productId).unwrap();
      toast.success("Sản phẩm đã được xóa thành công!");
      setDeletingId(null);
      refetch();
    } catch (error: any) {
      toast.error(error?.data?.message || "Có lỗi xảy ra khi xóa sản phẩm");
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
              Có lỗi xảy ra khi tải dữ liệu sản phẩm
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Quản lý sản phẩm</h1>
        <Dialog open={isCreateModalOpen} onOpenChange={setIsCreateModalOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="w-4 h-4 mr-2" />
              Thêm sản phẩm
            </Button>
          </DialogTrigger>
          <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>Thêm sản phẩm mới</DialogTitle>
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
          <div className="relative">
            <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Tìm kiếm sản phẩm..."
              value={searchTerm}
              onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                setSearchTerm(e.target.value)
              }
              className="pl-10"
            />
          </div>
        </CardContent>
      </Card>

      {/* Products Table */}
      <Card>
        <CardHeader>
          <CardTitle>Sản phẩm ({products.length})</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-8">Đang tải...</div>
          ) : products.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              Không có sản phẩm nào
            </div>
          ) : (
            <div className="border rounded-lg">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>ID</TableHead>
                    <TableHead>Tên sản phẩm</TableHead>
                    <TableHead>Danh mục</TableHead>
                    <TableHead>Thương hiệu</TableHead>
                    <TableHead>Giá</TableHead>
                    <TableHead>Tồn kho</TableHead>
                    <TableHead>Trạng thái</TableHead>
                    <TableHead>Thao tác</TableHead>
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
                          {product.status ? "Hoạt động" : "Ẩn"}
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
                                  Xác nhận xóa
                                </AlertDialogTitle>
                                <AlertDialogDescription>
                                  Bạn có chắc chắn muốn xóa sản phẩm "
                                  {product.name}"? Hành động này không thể hoàn
                                  tác.
                                </AlertDialogDescription>
                              </AlertDialogHeader>
                              <AlertDialogFooter>
                                <AlertDialogCancel>Hủy</AlertDialogCancel>
                                <AlertDialogAction
                                  onClick={() =>
                                    handleDeleteProduct(product.id)
                                  }
                                  disabled={isDeleting}
                                  className="bg-red-600 hover:bg-red-700"
                                >
                                  {isDeleting ? "Đang xóa..." : "Xóa"}
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
            <DialogTitle>Thêm sản phẩm mới</DialogTitle>
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
            <DialogTitle>Sửa sản phẩm</DialogTitle>
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
          <Label htmlFor="name">Tên sản phẩm *</Label>
          <Input
            id="name"
            value={formData.name}
            onChange={handleNameChange}
            required
            placeholder="Nhập tên sản phẩm..."
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
            placeholder="slug-tu-dong-tao"
            className="bg-gray-50"
          />
          <p className="text-xs text-muted-foreground">
            Slug sẽ được tạo tự động từ tên sản phẩm
          </p>
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="description">Mô tả</Label>
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
          <Label htmlFor="origin">Xuất xứ</Label>
          <Input
            id="origin"
            value={formData.origin}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("origin", e.target.value)
            }
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="volume">Dung tích (L) *</Label>
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
          <Label htmlFor="alcoholContent">Nồng độ cồn (%) *</Label>
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
          <Label htmlFor="price">Giá (VND) *</Label>
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
          <Label htmlFor="stockQuantity">Số lượng tồn kho *</Label>
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
          <Label htmlFor="categoryId">Danh mục *</Label>
          <Select
            value={formData.categoryId ? formData.categoryId.toString() : ""}
            onValueChange={(value) =>
              handleInputChange("categoryId", parseInt(value))
            }
          >
            <SelectTrigger>
              <SelectValue placeholder="Chọn danh mục" />
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
          <Label htmlFor="brandId">Thương hiệu *</Label>
          <Select
            value={formData.brandId ? formData.brandId.toString() : ""}
            onValueChange={(value) =>
              handleInputChange("brandId", parseInt(value))
            }
          >
            <SelectTrigger>
              <SelectValue placeholder="Chọn thương hiệu" />
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
          <Label htmlFor="imageUrl">URL hình ảnh</Label>
          <Input
            id="imageUrl"
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
        <Label htmlFor="status">Trạng thái hoạt động</Label>
      </div>

      <div className="flex justify-end space-x-2 pt-4">
        <Button type="submit" disabled={isLoading}>
          {isLoading
            ? "Đang lưu..."
            : initialData
            ? "Cập nhật"
            : "Tạo sản phẩm"}
        </Button>
      </div>
    </form>
  );
}
