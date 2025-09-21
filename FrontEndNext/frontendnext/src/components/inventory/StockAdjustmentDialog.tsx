"use client";

import React, { useState, useEffect } from "react";
import {
  useAdjustStockMutation,
  useUpdateStockMutation,
} from "@/lib/services/inventoryService";
import { Inventory, StockAdjustmentReason } from "@/lib/types/inventory";
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
import { Badge } from "@/components/ui/badge";
import { toast } from "sonner";
import {
  Settings,
  TrendingUp,
  TrendingDown,
  Package,
  AlertTriangle,
  Plus,
  RotateCcw,
  Trash2,
  Clock,
  Shield,
  BarChart3,
  AlertCircle,
  PackageX,
  FileText,
} from "lucide-react";
import { formatCurrency } from "@/lib/utils";

interface StockAdjustmentDialogProps {
  open: boolean;
  onClose: () => void;
  inventory: Inventory | null;
  onSuccess?: () => void;
}

interface AdjustmentFormData {
  quantity: number;
  reason: StockAdjustmentReason;
  notes: string;
}

interface UpdateFormData {
  quantity: number;
}

export function StockAdjustmentDialog({
  open,
  onClose,
  inventory,
  onSuccess,
}: StockAdjustmentDialogProps) {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [activeTab, setActiveTab] = useState("adjust");

  const [adjustmentForm, setAdjustmentForm] = useState<AdjustmentFormData>({
    quantity: 0,
    reason: StockAdjustmentReason.Other,
    notes: "",
  });

  const [updateForm, setUpdateForm] = useState<UpdateFormData>({
    quantity: 0,
  });

  const [adjustStock] = useAdjustStockMutation();
  const [updateStock] = useUpdateStockMutation();

  // Reset forms when inventory changes
  useEffect(() => {
    if (inventory) {
      setAdjustmentForm({
        quantity: 0,
        reason: StockAdjustmentReason.Other,
        notes: "",
      });
      setUpdateForm({
        quantity: inventory.quantity,
      });
    }
  }, [inventory]);

  const onAdjustmentSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!inventory) return;

    // Validation
    if (adjustmentForm.quantity === 0) {
      toast.error("Adjustment quantity cannot be zero");
      return;
    }

    if (!adjustmentForm.reason) {
      toast.error("Please select an adjustment reason");
      return;
    }

    if (
      adjustmentForm.reason === StockAdjustmentReason.Other &&
      !adjustmentForm.notes.trim()
    ) {
      toast.error("Please provide notes when selecting 'Other' reason");
      return;
    }

    // Prevent negative stock
    const newQuantity = inventory.quantity + adjustmentForm.quantity;
    if (newQuantity < 0) {
      toast.error(
        `Cannot reduce stock below 0. Maximum reduction: ${inventory.quantity}`
      );
      return;
    }

    setIsSubmitting(true);
    try {
      await adjustStock({
        productId: inventory.productId,
        request: {
          quantity: adjustmentForm.quantity,
          reason: adjustmentForm.reason,
          notes: adjustmentForm.notes,
        },
      }).unwrap();

      toast.success("Stock adjusted successfully");
      onSuccess?.();
      onClose();
    } catch (error: any) {
      toast.error(error?.data?.message || "Failed to adjust stock");
    } finally {
      setIsSubmitting(false);
    }
  };

  const onUpdateSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!inventory) return;

    setIsSubmitting(true);
    try {
      await updateStock({
        id: inventory.id,
        request: {
          quantity: updateForm.quantity,
        },
      }).unwrap();

      toast.success("Stock updated successfully");
      onSuccess?.();
      onClose();
    } catch (error: any) {
      toast.error(error?.data?.message || "Failed to update stock");
    } finally {
      setIsSubmitting(false);
    }
  };

  const getStockStatus = (quantity: number) => {
    if (quantity === 0) {
      return {
        label: "Out of Stock",
        variant: "destructive" as const,
        icon: AlertTriangle,
      };
    }
    if (inventory?.reorderLevel && quantity <= inventory.reorderLevel) {
      return {
        label: "Low Stock",
        variant: "secondary" as const,
        icon: TrendingDown,
      };
    }
    return { label: "In Stock", variant: "default" as const, icon: TrendingUp };
  };

  if (!inventory) return null;

  const currentStatus = getStockStatus(inventory.quantity);
  const CurrentStatusIcon = currentStatus?.icon;

  // Calculate new quantity for adjustment preview
  const newQuantityFromAdjustment =
    inventory.quantity + adjustmentForm.quantity;
  const newStatus = getStockStatus(newQuantityFromAdjustment);
  const NewStatusIcon = newStatus.icon;

  // Calculate new quantity for update preview
  const updateStatus = getStockStatus(updateForm.quantity);
  const UpdateStatusIcon = updateStatus.icon;

  return (
    <Dialog open={open} onOpenChange={onClose}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <Settings className="h-5 w-5" />
            Stock Management - {inventory.productName}
          </DialogTitle>
        </DialogHeader>

        {/* Workflow Warning */}
        <div className="bg-red-50 dark:bg-red-950/20 border border-red-200 dark:border-red-800 rounded-lg p-3">
          <div className="flex items-start gap-2">
            <AlertTriangle className="h-4 w-4 text-red-600 mt-0.5" />
            <div>
              <p className="text-xs font-medium text-red-800 dark:text-red-200">
                <Shield className="h-3 w-3 inline mr-1" />
                CRITICAL: Stock Adjustment Business Rules
              </p>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-3 mt-2">
                <div>
                  <p className="text-xs font-semibold text-red-800 dark:text-red-200">
                    ALLOWED:
                  </p>
                  <ul className="text-xs text-red-700 dark:text-red-300 space-y-1">
                    <li>• Damaged/Expired goods removal</li>
                    <li>• Theft/Loss reporting</li>
                    <li>• Inventory count corrections</li>
                    <li>• Quality issue removals</li>
                    <li>• Return to Supplier (with proper documentation)</li>
                  </ul>
                </div>
                <div>
                  <p className="text-xs font-semibold text-red-800 dark:text-red-200">
                    FORBIDDEN:
                  </p>
                  <ul className="text-xs text-red-700 dark:text-red-300 space-y-1">
                    <li>• Regular stock additions (use Import Orders)</li>
                    <li>• Sales transactions (use Order system)</li>
                    <li>• Arbitrary adjustments without reason</li>
                    <li>• Adjustments without proper documentation</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* Current Stock Info */}
        <Card>
          <CardHeader>
            <CardTitle className="text-sm flex items-center gap-2">
              <Package className="h-4 w-4" />
              Current Stock Information
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div>
                <p className="text-sm text-muted-foreground">
                  Current Quantity
                </p>
                <p className="text-2xl font-bold">{inventory.quantity}</p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Average Cost</p>
                <p className="text-lg font-semibold">
                  {formatCurrency(inventory.averageCost)}
                </p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Total Value</p>
                <p className="text-lg font-semibold text-green-600">
                  {formatCurrency(inventory.totalValue)}
                </p>
              </div>
              <div>
                <p className="text-sm text-muted-foreground">Status</p>
                {currentStatus && CurrentStatusIcon && (
                  <Badge
                    variant={currentStatus.variant}
                    className="flex items-center gap-1 w-fit"
                  >
                    <CurrentStatusIcon className="h-3 w-3" />
                    {currentStatus.label}
                  </Badge>
                )}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Tab Navigation */}
        <div className="flex border-b">
          <button
            className={`px-4 py-2 font-medium text-sm ${
              activeTab === "adjust"
                ? "border-b-2 border-blue-500 text-blue-600"
                : "text-gray-500 hover:text-gray-700"
            }`}
            onClick={() => setActiveTab("adjust")}
          >
            <Plus className="h-4 w-4 inline mr-1" />
            Adjust Stock
          </button>
          <button
            className={`px-4 py-2 font-medium text-sm ${
              activeTab === "update"
                ? "border-b-2 border-blue-500 text-blue-600"
                : "text-gray-500 hover:text-gray-700"
            }`}
            onClick={() => setActiveTab("update")}
          >
            <RotateCcw className="h-4 w-4 inline mr-1" />
            Set Stock
          </button>
        </div>

        {/* Stock Adjustment Tab */}
        {activeTab === "adjust" && (
          <form onSubmit={onAdjustmentSubmit} className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle className="text-sm">Stock Adjustment</CardTitle>
                <p className="text-sm text-muted-foreground">
                  Add or remove items from current stock. Use negative numbers
                  to reduce stock.
                </p>
              </CardHeader>
              <CardContent className="space-y-4">
                {/* Reason Selection */}
                <div className="space-y-2">
                  <Label htmlFor="adjustment-reason">Adjustment Reason *</Label>
                  <Select
                    value={adjustmentForm.reason}
                    onValueChange={(value: StockAdjustmentReason) =>
                      setAdjustmentForm((prev) => ({ ...prev, reason: value }))
                    }
                  >
                    <SelectTrigger>
                      <SelectValue placeholder="Select adjustment reason" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value={StockAdjustmentReason.Damaged}>
                        <div className="flex items-center gap-2">
                          <Trash2 className="h-3 w-3 text-red-500" />
                          Damaged Goods
                        </div>
                      </SelectItem>
                      <SelectItem value={StockAdjustmentReason.Expired}>
                        <div className="flex items-center gap-2">
                          <Clock className="h-3 w-3 text-orange-500" />
                          Expired Products
                        </div>
                      </SelectItem>
                      <SelectItem value={StockAdjustmentReason.Theft}>
                        <div className="flex items-center gap-2">
                          <Shield className="h-3 w-3 text-red-600" />
                          Theft/Loss
                        </div>
                      </SelectItem>
                      <SelectItem value={StockAdjustmentReason.CountCorrection}>
                        <div className="flex items-center gap-2">
                          <BarChart3 className="h-3 w-3 text-blue-500" />
                          Inventory Count Correction
                        </div>
                      </SelectItem>
                      <SelectItem value={StockAdjustmentReason.QualityIssue}>
                        <div className="flex items-center gap-2">
                          <AlertCircle className="h-3 w-3 text-orange-600" />
                          Quality Issues
                        </div>
                      </SelectItem>
                      <SelectItem
                        value={StockAdjustmentReason.ReturnToSupplier}
                      >
                        <div className="flex items-center gap-2">
                          <PackageX className="h-3 w-3 text-purple-500" />
                          Return to Supplier
                        </div>
                      </SelectItem>
                      <SelectItem value={StockAdjustmentReason.Other}>
                        <div className="flex items-center gap-2">
                          <FileText className="h-3 w-3 text-gray-500" />
                          Other (Specify in notes)
                        </div>
                      </SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="adjustment-quantity">
                      Adjustment Quantity
                    </Label>
                    <Input
                      id="adjustment-quantity"
                      type="number"
                      step="1"
                      placeholder="Enter adjustment (+/-)"
                      value={adjustmentForm.quantity}
                      onChange={(e) =>
                        setAdjustmentForm((prev) => ({
                          ...prev,
                          quantity: parseInt(e.target.value) || 0,
                        }))
                      }
                    />
                    <div className="flex gap-2">
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() =>
                          setAdjustmentForm((prev) => ({
                            ...prev,
                            quantity: 10,
                          }))
                        }
                      >
                        +10
                      </Button>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() =>
                          setAdjustmentForm((prev) => ({
                            ...prev,
                            quantity: -10,
                          }))
                        }
                      >
                        -10
                      </Button>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() =>
                          setAdjustmentForm((prev) => ({
                            ...prev,
                            quantity: 0,
                          }))
                        }
                      >
                        Reset
                      </Button>
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label>Preview</Label>
                    <div className="p-3 border rounded-lg bg-muted/50">
                      <div className="space-y-1">
                        <p className="text-sm">
                          <span className="font-medium">Current:</span>{" "}
                          {inventory.quantity}
                        </p>
                        <p className="text-sm">
                          <span className="font-medium">Adjustment:</span>{" "}
                          {adjustmentForm.quantity >= 0 ? "+" : ""}
                          {adjustmentForm.quantity}
                        </p>
                        <p className="text-sm font-semibold">
                          <span className="font-medium">New Total:</span>{" "}
                          {newQuantityFromAdjustment}
                        </p>
                        <Badge
                          variant={newStatus.variant}
                          className="flex items-center gap-1 w-fit"
                        >
                          <NewStatusIcon className="h-3 w-3" />
                          {newStatus.label}
                        </Badge>
                      </div>
                    </div>
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="adjustment-notes">Notes</Label>
                  <Textarea
                    id="adjustment-notes"
                    placeholder="Reason for stock adjustment..."
                    rows={3}
                    value={adjustmentForm.notes}
                    onChange={(e) =>
                      setAdjustmentForm((prev) => ({
                        ...prev,
                        notes: e.target.value,
                      }))
                    }
                  />
                </div>
              </CardContent>
            </Card>

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
                {isSubmitting ? "Adjusting..." : "Adjust Stock"}
              </Button>
            </div>
          </form>
        )}

        {/* Stock Update Tab */}
        {activeTab === "update" && (
          <form onSubmit={onUpdateSubmit} className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle className="text-sm">Set Stock Quantity</CardTitle>
                <p className="text-sm text-muted-foreground">
                  Set the absolute stock quantity. This will replace the current
                  quantity.
                </p>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div className="space-y-2">
                    <Label htmlFor="update-quantity">New Quantity</Label>
                    <Input
                      id="update-quantity"
                      type="number"
                      min="0"
                      step="1"
                      placeholder="Enter new quantity"
                      value={updateForm.quantity}
                      onChange={(e) =>
                        setUpdateForm((prev) => ({
                          ...prev,
                          quantity: parseInt(e.target.value) || 0,
                        }))
                      }
                    />
                    <div className="flex gap-2">
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() =>
                          setUpdateForm((prev) => ({ ...prev, quantity: 0 }))
                        }
                      >
                        0
                      </Button>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() =>
                          setUpdateForm((prev) => ({ ...prev, quantity: 50 }))
                        }
                      >
                        50
                      </Button>
                      <Button
                        type="button"
                        variant="outline"
                        size="sm"
                        onClick={() =>
                          setUpdateForm((prev) => ({ ...prev, quantity: 100 }))
                        }
                      >
                        100
                      </Button>
                    </div>
                  </div>

                  <div className="space-y-2">
                    <Label>Preview</Label>
                    <div className="p-3 border rounded-lg bg-muted/50">
                      <div className="space-y-1">
                        <p className="text-sm">
                          <span className="font-medium">Current:</span>{" "}
                          {inventory.quantity}
                        </p>
                        <p className="text-sm">
                          <span className="font-medium">Change:</span>{" "}
                          {updateForm.quantity - inventory.quantity >= 0
                            ? "+"
                            : ""}
                          {updateForm.quantity - inventory.quantity}
                        </p>
                        <p className="text-sm font-semibold">
                          <span className="font-medium">New Total:</span>{" "}
                          {updateForm.quantity}
                        </p>
                        <Badge
                          variant={updateStatus.variant}
                          className="flex items-center gap-1 w-fit"
                        >
                          <UpdateStatusIcon className="h-3 w-3" />
                          {updateStatus.label}
                        </Badge>
                      </div>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>

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
                {isSubmitting ? "Updating..." : "Update Stock"}
              </Button>
            </div>
          </form>
        )}
      </DialogContent>
    </Dialog>
  );
}

export default StockAdjustmentDialog;
