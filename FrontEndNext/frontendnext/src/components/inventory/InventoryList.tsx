"use client";

import React, { useState, useMemo } from "react";
import {
  useGetAllInventoryQuery,
  useDeleteInventoryMutation,
  useGetLowStockItemsQuery,
  useGetTotalInventoryValueQuery,
} from "@/lib/services/inventoryService";
import { Inventory, InventoryFilterDto } from "@/lib/types/inventory";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
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
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { SearchInput } from "@/components/admin/SearchInput";
import {
  Package,
  AlertTriangle,
  DollarSign,
  Edit,
  Trash2,
  Plus,
  TrendingUp,
  TrendingDown,
  Eye,
  Settings,
} from "lucide-react";
import { toast } from "sonner";
import { formatCurrency } from "@/lib/utils";

interface InventoryListProps {
  onEdit?: (inventory: Inventory) => void;
  onView?: (inventory: Inventory) => void;
  onCreate?: () => void;
  onStockAdjust?: (inventory: Inventory) => void;
}

export function InventoryList({
  onEdit,
  onView,
  onCreate,
  onStockAdjust,
}: InventoryListProps) {
  const [filter, setFilter] = useState<InventoryFilterDto>({
    page: 1,
    pageSize: 20,
    sortBy: "productName",
    sortOrder: "asc",
  });

  const [searchTerm, setSearchTerm] = useState("");

  const {
    data: inventoryData,
    isLoading,
    error,
  } = useGetAllInventoryQuery(filter);

  const { data: lowStockItems } = useGetLowStockItemsQuery();
  const { data: totalValue } = useGetTotalInventoryValueQuery();

  const [deleteInventory] = useDeleteInventoryMutation();

  // Filter inventory based on search term
  const filteredInventory = useMemo(() => {
    if (!inventoryData?.items || !searchTerm) {
      return inventoryData?.items || [];
    }

    return inventoryData.items.filter(
      (item) =>
        item.productName.toLowerCase().includes(searchTerm.toLowerCase()) ||
        item.id.toString().includes(searchTerm)
    );
  }, [inventoryData?.items, searchTerm]);

  const handleDelete = async (id: number) => {
    try {
      await deleteInventory(id).unwrap();
      toast.success("Inventory deleted successfully");
    } catch (error) {
      toast.error("Failed to delete inventory");
    }
  };

  const handleFilterChange = (key: keyof InventoryFilterDto, value: any) => {
    setFilter((prev) => ({ ...prev, [key]: value, page: 1 }));
  };

  const getStockStatus = (item: Inventory) => {
    if (item.quantity === 0) {
      return {
        label: "Out of Stock",
        variant: "destructive" as const,
        icon: AlertTriangle,
      };
    }
    if (item.reorderLevel && item.quantity <= item.reorderLevel) {
      return {
        label: "Low Stock",
        variant: "secondary" as const,
        icon: TrendingDown,
      };
    }
    return { label: "In Stock", variant: "default" as const, icon: TrendingUp };
  };

  if (error) {
    return (
      <Card>
        <CardContent className="p-6">
          <div className="text-center text-red-500">
            Error loading inventory data
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Items</CardTitle>
            <Package className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {inventoryData?.totalItems || 0}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">
              Low Stock Items
            </CardTitle>
            <AlertTriangle className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-orange-600">
              {lowStockItems?.length || 0}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Value</CardTitle>
            <DollarSign className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold text-green-600">
              {formatCurrency(totalValue || 0)}
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Filters and Actions */}
      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <CardTitle>Inventory Management</CardTitle>
            {onCreate && (
              <Button onClick={onCreate} className="flex items-center gap-2">
                <Plus className="h-4 w-4" />
                Add Inventory
              </Button>
            )}
          </div>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col sm:flex-row gap-4 mb-6">
            <SearchInput
              value={searchTerm}
              onChange={setSearchTerm}
              placeholder="Search by product name or ID..."
              className="flex-1"
            />

            <div className="flex gap-2">
              <Select
                value={filter.sortBy}
                onValueChange={(value) => handleFilterChange("sortBy", value)}
              >
                <SelectTrigger className="w-40">
                  <SelectValue placeholder="Sort by" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="productName">Product Name</SelectItem>
                  <SelectItem value="quantity">Quantity</SelectItem>
                  <SelectItem value="totalValue">Total Value</SelectItem>
                  <SelectItem value="lastUpdated">Last Updated</SelectItem>
                </SelectContent>
              </Select>

              <Select
                value={filter.sortOrder}
                onValueChange={(value: "asc" | "desc") =>
                  handleFilterChange("sortOrder", value)
                }
              >
                <SelectTrigger className="w-32">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="asc">Ascending</SelectItem>
                  <SelectItem value="desc">Descending</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Inventory Table */}
          <div className="border rounded-lg">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Product</TableHead>
                  <TableHead>Quantity</TableHead>
                  <TableHead>Average Cost</TableHead>
                  <TableHead>Total Value</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Last Updated</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {isLoading ? (
                  <TableRow>
                    <TableCell colSpan={7} className="text-center py-8">
                      Loading...
                    </TableCell>
                  </TableRow>
                ) : filteredInventory.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={7} className="text-center py-8">
                      No inventory items found
                    </TableCell>
                  </TableRow>
                ) : (
                  filteredInventory.map((item) => {
                    const status = getStockStatus(item);
                    const StatusIcon = status.icon;

                    return (
                      <TableRow key={item.id}>
                        <TableCell className="font-medium">
                          {item.productName}
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center gap-2">
                            {item.quantity}
                            {item.reorderLevel &&
                              item.quantity <= item.reorderLevel && (
                                <AlertTriangle className="h-4 w-4 text-orange-500" />
                              )}
                          </div>
                        </TableCell>
                        <TableCell>
                          {formatCurrency(item.averageCost)}
                        </TableCell>
                        <TableCell className="font-medium">
                          {formatCurrency(item.totalValue)}
                        </TableCell>
                        <TableCell>
                          <Badge
                            variant={status.variant}
                            className="flex items-center gap-1 w-fit"
                          >
                            <StatusIcon className="h-3 w-3" />
                            {status.label}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          {new Date(item.lastUpdated).toLocaleDateString()}
                        </TableCell>
                        <TableCell className="text-right">
                          <div className="flex items-center justify-end gap-2">
                            {onView && (
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => onView(item)}
                              >
                                <Eye className="h-4 w-4" />
                              </Button>
                            )}
                            {onStockAdjust && (
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => onStockAdjust(item)}
                              >
                                <Settings className="h-4 w-4" />
                              </Button>
                            )}
                            {onEdit && (
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => onEdit(item)}
                              >
                                <Edit className="h-4 w-4" />
                              </Button>
                            )}
                            <AlertDialog>
                              <AlertDialogTrigger asChild>
                                <Button variant="ghost" size="sm">
                                  <Trash2 className="h-4 w-4" />
                                </Button>
                              </AlertDialogTrigger>
                              <AlertDialogContent>
                                <AlertDialogHeader>
                                  <AlertDialogTitle>
                                    Delete Inventory
                                  </AlertDialogTitle>
                                  <AlertDialogDescription>
                                    Are you sure you want to delete this
                                    inventory item for "{item.productName}"?
                                    This action cannot be undone.
                                  </AlertDialogDescription>
                                </AlertDialogHeader>
                                <AlertDialogFooter>
                                  <AlertDialogCancel>Cancel</AlertDialogCancel>
                                  <AlertDialogAction
                                    onClick={() => handleDelete(item.id)}
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
                    );
                  })
                )}
              </TableBody>
            </Table>
          </div>

          {/* Pagination */}
          {inventoryData && inventoryData.totalPages > 1 && (
            <div className="flex justify-between items-center mt-4">
              <div className="text-sm text-muted-foreground">
                Showing {((filter.page || 1) - 1) * (filter.pageSize || 20) + 1}{" "}
                to{" "}
                {Math.min(
                  (filter.page || 1) * (filter.pageSize || 20),
                  inventoryData.totalItems
                )}{" "}
                of {inventoryData.totalItems} items
              </div>
              <div className="flex gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  disabled={(filter.page || 1) <= 1}
                  onClick={() =>
                    handleFilterChange("page", (filter.page || 1) - 1)
                  }
                >
                  Previous
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  disabled={(filter.page || 1) >= inventoryData.totalPages}
                  onClick={() =>
                    handleFilterChange("page", (filter.page || 1) + 1)
                  }
                >
                  Next
                </Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}

export default InventoryList;
