"use client";

import React, { useState, useEffect } from "react";
import {
  useCreateInventoryMutation,
  useUpdateInventoryMutation,
} from "@/lib/services/inventoryService";
import { useGetProductsQuery } from "@/lib/services/productService";
import {
  Inventory,
  InventoryCreateDto,
  InventoryUpdateDto,
} from "@/lib/types/inventory";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";
import { toast } from "sonner";
import { Package, DollarSign, Hash, FileText } from "lucide-react";

interface InventoryFormProps {
  open: boolean;
  onClose: () => void;
  inventory?: Inventory | null;
  onSuccess?: () => void;
}

interface FormData {
  productId: number;
  quantity: number;
  notes: string;
}

export function InventoryForm({
  open,
  onClose,
  inventory,
  onSuccess,
}: InventoryFormProps) {
  const isEditing = !!inventory;
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formData, setFormData] = useState<FormData>({
    productId: 0,
    quantity: 0,
    notes: "",
  });

  const { data: productsData } = useGetProductsQuery({
    pageNumber: 1,
    pageSize: 1000,
  });
  const [createInventory] = useCreateInventoryMutation();
  const [updateInventory] = useUpdateInventoryMutation();

  const totalValue = inventory ? inventory.quantity * inventory.averageCost : 0;

  // Reset form when dialog opens/closes or inventory changes
  useEffect(() => {
    if (open) {
      if (inventory) {
        setFormData({
          productId: inventory.productId,
          quantity: inventory.quantity,
          notes: inventory.notes || "",
        });
      } else {
        setFormData({
          productId: 0,
          quantity: 0,
          notes: "",
        });
      }
    }
  }, [open, inventory]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (formData.productId === 0) {
      toast.error("Please select a product");
      return;
    }

    if (formData.quantity < 0) {
      toast.error("Quantity must be 0 or greater");
      return;
    }

    setIsSubmitting(true);
    try {
      if (isEditing && inventory) {
        const updateData: InventoryUpdateDto = {
          productId: formData.productId,
          quantity: formData.quantity,
          notes: formData.notes,
        };
        await updateInventory({
          id: inventory.id,
          inventory: updateData,
        }).unwrap();
        toast.success("Inventory updated successfully");
      } else {
        const createData: InventoryCreateDto = {
          productId: formData.productId,
          quantity: formData.quantity,
          notes: formData.notes,
        };
        await createInventory(createData).unwrap();
        toast.success("Inventory created successfully");
      }

      onSuccess?.();
      onClose();
    } catch (error: any) {
      toast.error(error?.data?.message || "An error occurred");
    } finally {
      setIsSubmitting(false);
    }
  };

  const getSelectedProduct = () => {
    if (!productsData?.items || !formData.productId) return null;
    return productsData.items.find((p: any) => p.id === formData.productId);
  };

  const selectedProduct = getSelectedProduct();

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Package className="h-5 w-5" />
            {isEditing ? "Edit Inventory" : "Create New Inventory"}
          </DialogTitle>
        </DialogHeader>

        {/* Workflow Information */}
        {!isEditing && (
          <div className="bg-blue-50 dark:bg-blue-950/20 border border-blue-200 dark:border-blue-800 rounded-lg p-4">
            <div className="flex items-start gap-3">
              <Package className="h-5 w-5 text-blue-600 mt-0.5" />
              <div className="space-y-1">
                <p className="text-sm font-medium text-blue-800 dark:text-blue-200">
                  Inventory Creation Workflow
                </p>
                <ul className="text-sm text-blue-700 dark:text-blue-300 space-y-1">
                  <li>
                    • New inventory items are created with AverageCost = 0
                  </li>
                  <li>
                    • AverageCost is calculated automatically when completing
                    Import Orders
                  </li>
                  <li>
                    • Recommended: Set initial quantity = 0, then use Import
                    Orders to add stock
                  </li>
                </ul>
              </div>
            </div>
          </div>
        )}

        <form onSubmit={handleSubmit} className="space-y-6">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
            {/* Product Selection */}
            <div className="space-y-2">
              <Label htmlFor="productId" className="flex items-center gap-2">
                <Package className="h-4 w-4" />
                Product *
              </Label>
              <Select
                value={formData.productId?.toString() || ""}
                onValueChange={(value) =>
                  setFormData((prev) => ({
                    ...prev,
                    productId: parseInt(value),
                  }))
                }
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select a product" />
                </SelectTrigger>
                <SelectContent>
                  {productsData?.items?.map((product: any) => (
                    <SelectItem key={product.id} value={product.id.toString()}>
                      {product.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>

            {/* Quantity */}
            <div className="space-y-2">
              <Label htmlFor="quantity" className="flex items-center gap-2">
                <Hash className="h-4 w-4" />
                Quantity *
              </Label>
              <Input
                id="quantity"
                type="number"
                min="0"
                step="1"
                placeholder="Enter quantity"
                value={formData.quantity}
                onChange={(e) =>
                  setFormData((prev) => ({
                    ...prev,
                    quantity: parseInt(e.target.value) || 0,
                  }))
                }
              />
            </div>

            {/* Average Cost & Total Value (Read-only for editing) */}
            {isEditing && (
              <>
                <div className="space-y-2">
                  <Label className="flex items-center gap-2">
                    <DollarSign className="h-4 w-4" />
                    Average Cost
                  </Label>
                  <Input
                    value={inventory?.averageCost?.toFixed(2) || "0.00"}
                    disabled
                    className="bg-muted"
                  />
                  <p className="text-xs text-muted-foreground">
                    Calculated automatically from Import Orders
                  </p>
                </div>

                <div className="space-y-2">
                  <Label className="flex items-center gap-2">
                    <DollarSign className="h-4 w-4" />
                    Total Value
                  </Label>
                  <Input
                    value={totalValue.toFixed(2)}
                    disabled
                    className="bg-muted"
                  />
                  <p className="text-xs text-muted-foreground">
                    Automatically calculated: Quantity × Average Cost
                  </p>
                </div>
              </>
            )}
          </div>

          {/* Selected Product Info */}
          {selectedProduct && (
            <Card>
              <CardHeader>
                <CardTitle className="text-sm">Product Information</CardTitle>
              </CardHeader>
              <CardContent className="space-y-2">
                <div className="grid grid-cols-2 gap-4 text-sm">
                  <div>
                    <span className="font-medium">Name:</span>{" "}
                    {selectedProduct.name}
                  </div>
                  <div>
                    <span className="font-medium">Current Stock:</span>{" "}
                    {selectedProduct.stockQuantity}
                  </div>
                  <div>
                    <span className="font-medium">Price:</span> $
                    {selectedProduct.price?.toFixed(2)}
                  </div>
                  <div>
                    <span className="font-medium">Category:</span>{" "}
                    {selectedProduct.categoryName || "N/A"}
                  </div>
                </div>
              </CardContent>
            </Card>
          )}

          {/* Notes */}
          <div className="space-y-2">
            <Label htmlFor="notes" className="flex items-center gap-2">
              <FileText className="h-4 w-4" />
              Notes
            </Label>
            <Textarea
              id="notes"
              placeholder="Enter any additional notes..."
              rows={3}
              value={formData.notes}
              onChange={(e) =>
                setFormData((prev) => ({ ...prev, notes: e.target.value }))
              }
            />
          </div>

          {/* Form Actions */}
          <div className="flex justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              onClick={onClose}
              disabled={isSubmitting}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting
                ? isEditing
                  ? "Updating..."
                  : "Creating..."
                : isEditing
                ? "Update Inventory"
                : "Create Inventory"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export default InventoryForm;
