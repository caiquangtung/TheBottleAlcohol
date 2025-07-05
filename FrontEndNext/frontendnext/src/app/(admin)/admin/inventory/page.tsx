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
  useGetAllInventoryQuery,
  useCreateInventoryMutation,
  useUpdateInventoryMutation,
  useDeleteInventoryMutation,
} from "@/lib/services/inventoryService";
import { useGetProductsQuery } from "@/lib/services/productService";
import {
  Inventory,
  InventoryCreateDto,
  InventoryUpdateDto,
} from "@/lib/types/inventory";
import { Product } from "@/lib/types/product";

export default function InventoryPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [isUpdateDialogOpen, setIsUpdateDialogOpen] = useState(false);
  const [selectedInventory, setSelectedInventory] = useState<Inventory | null>(
    null
  );
  const [createForm, setCreateForm] = useState<InventoryCreateDto>({
    productId: 0,
    quantity: 0,
    reorderLevel: 0,
    location: "",
    notes: "",
  });
  const [updateForm, setUpdateForm] = useState<InventoryUpdateDto>({
    productId: 0,
    quantity: 0,
    reorderLevel: 0,
    location: "",
    notes: "",
  });

  // RTK Query hooks
  const { data: inventory = [], isLoading, error } = useGetAllInventoryQuery();
  const { data: productsData } = useGetProductsQuery({
    pageNumber: 1,
    pageSize: 1000,
  });
  const products: Product[] = productsData?.items || [];
  const [createInventory, { isLoading: isCreating }] =
    useCreateInventoryMutation();
  const [updateInventory, { isLoading: isUpdating }] =
    useUpdateInventoryMutation();
  const [deleteInventory, { isLoading: isDeleting }] =
    useDeleteInventoryMutation();

  const handleCreate = async () => {
    try {
      await createInventory(createForm).unwrap();
      toast.success("Inventory created successfully");
      setIsCreateDialogOpen(false);
      setCreateForm({
        productId: 0,
        quantity: 0,
        reorderLevel: 0,
        location: "",
        notes: "",
      });
    } catch (error) {
      toast.error("Failed to create inventory");
      console.error("Error creating inventory:", error);
    }
  };

  const handleUpdate = async () => {
    if (!selectedInventory) return;

    try {
      await updateInventory({
        id: selectedInventory.id,
        inventory: updateForm,
      }).unwrap();
      toast.success("Inventory updated successfully");
      setIsUpdateDialogOpen(false);
      setSelectedInventory(null);
      setUpdateForm({
        productId: 0,
        quantity: 0,
        reorderLevel: 0,
        location: "",
        notes: "",
      });
    } catch (error) {
      toast.error("Failed to update inventory");
      console.error("Error updating inventory:", error);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteInventory(id).unwrap();
      toast.success("Inventory deleted successfully");
    } catch (error) {
      toast.error("Failed to delete inventory");
      console.error("Error deleting inventory:", error);
    }
  };

  const openUpdateDialog = (inventoryItem: Inventory) => {
    setSelectedInventory(inventoryItem);
    setUpdateForm({
      productId: inventoryItem.productId,
      quantity: inventoryItem.quantity,
      reorderLevel: inventoryItem.reorderLevel,
      location: inventoryItem.location,
      notes: inventoryItem.notes,
    });
    setIsUpdateDialogOpen(true);
  };

  const getStockStatus = (quantity: number, reorderLevel: number) => {
    if (quantity === 0)
      return <Badge variant="destructive">Out of Stock</Badge>;
    if (quantity <= reorderLevel)
      return <Badge variant="secondary">Low Stock</Badge>;
    return <Badge variant="default">In Stock</Badge>;
  };

  const getProductName = (productId: number) => {
    const product = products.find((p: Product) => p.id === productId);
    return product ? product.name : `Product ${productId}`;
  };

  const filteredInventory = inventory.filter(
    (item) =>
      getProductName(item.productId)
        .toLowerCase()
        .includes(searchTerm.toLowerCase()) ||
      item.location.toLowerCase().includes(searchTerm.toLowerCase()) ||
      item.notes.toLowerCase().includes(searchTerm.toLowerCase())
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
        <div className="text-red-500">Failed to load inventory</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Inventory Management</h1>
        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Add Inventory
            </Button>
          </DialogTrigger>
          <DialogContent>
            <DialogHeader>
              <DialogTitle>Create New Inventory</DialogTitle>
              <DialogDescription>
                Add a new inventory item to the system.
              </DialogDescription>
            </DialogHeader>
            <div className="space-y-4">
              <div>
                <Label htmlFor="productId">Product</Label>
                <select
                  id="productId"
                  value={createForm.productId}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      productId: parseInt(e.target.value),
                    })
                  }
                  className="w-full p-2 border rounded-md"
                >
                  <option value={0}>Select a product</option>
                  {products.map((product: Product) => (
                    <option key={product.id} value={product.id}>
                      {product.name}
                    </option>
                  ))}
                </select>
              </div>
              <div>
                <Label htmlFor="quantity">Quantity</Label>
                <Input
                  id="quantity"
                  type="number"
                  value={createForm.quantity}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      quantity: parseInt(e.target.value),
                    })
                  }
                  placeholder="0"
                />
              </div>
              <div>
                <Label htmlFor="reorderLevel">Reorder Level</Label>
                <Input
                  id="reorderLevel"
                  type="number"
                  value={createForm.reorderLevel}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      reorderLevel: parseInt(e.target.value),
                    })
                  }
                  placeholder="10"
                />
              </div>
              <div>
                <Label htmlFor="location">Location</Label>
                <Input
                  id="location"
                  value={createForm.location}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, location: e.target.value })
                  }
                  placeholder="Warehouse A, Shelf 1"
                />
              </div>
              <div>
                <Label htmlFor="notes">Notes</Label>
                <Textarea
                  id="notes"
                  value={createForm.notes}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, notes: e.target.value })
                  }
                  placeholder="Additional notes"
                />
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
            placeholder="Search inventory..."
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
              <TableHead>Product</TableHead>
              <TableHead>Quantity</TableHead>
              <TableHead>Reorder Level</TableHead>
              <TableHead>Location</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredInventory.map((item) => (
              <TableRow key={item.id}>
                <TableCell className="font-medium">
                  {getProductName(item.productId)}
                </TableCell>
                <TableCell>{item.quantity}</TableCell>
                <TableCell>{item.reorderLevel}</TableCell>
                <TableCell>{item.location}</TableCell>
                <TableCell>
                  {getStockStatus(item.quantity, item.reorderLevel)}
                </TableCell>
                <TableCell>
                  <div className="flex items-center space-x-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => openUpdateDialog(item)}
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
                          <AlertDialogTitle>Delete Inventory</AlertDialogTitle>
                          <AlertDialogDescription>
                            Are you sure you want to delete this inventory item?
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
            ))}
          </TableBody>
        </Table>
      </div>

      {/* Update Dialog */}
      <Dialog open={isUpdateDialogOpen} onOpenChange={setIsUpdateDialogOpen}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Update Inventory</DialogTitle>
            <DialogDescription>
              Update the inventory information.
            </DialogDescription>
          </DialogHeader>
          <div className="space-y-4">
            <div>
              <Label htmlFor="update-productId">Product</Label>
              <select
                id="update-productId"
                value={updateForm.productId}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    productId: parseInt(e.target.value),
                  })
                }
                className="w-full p-2 border rounded-md"
              >
                <option value={0}>Select a product</option>
                {products.map((product: Product) => (
                  <option key={product.id} value={product.id}>
                    {product.name}
                  </option>
                ))}
              </select>
            </div>
            <div>
              <Label htmlFor="update-quantity">Quantity</Label>
              <Input
                id="update-quantity"
                type="number"
                value={updateForm.quantity}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    quantity: parseInt(e.target.value),
                  })
                }
                placeholder="0"
              />
            </div>
            <div>
              <Label htmlFor="update-reorderLevel">Reorder Level</Label>
              <Input
                id="update-reorderLevel"
                type="number"
                value={updateForm.reorderLevel}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    reorderLevel: parseInt(e.target.value),
                  })
                }
                placeholder="10"
              />
            </div>
            <div>
              <Label htmlFor="update-location">Location</Label>
              <Input
                id="update-location"
                value={updateForm.location}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, location: e.target.value })
                }
                placeholder="Warehouse A, Shelf 1"
              />
            </div>
            <div>
              <Label htmlFor="update-notes">Notes</Label>
              <Textarea
                id="update-notes"
                value={updateForm.notes}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, notes: e.target.value })
                }
                placeholder="Additional notes"
              />
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
