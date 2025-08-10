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
  useGetAllDiscountsQuery,
  useCreateDiscountMutation,
  useUpdateDiscountMutation,
  useDeleteDiscountMutation,
} from "@/lib/services/discountService";
import {
  Discount,
  DiscountCreateDto,
  DiscountUpdateDto,
} from "@/lib/types/discount";

export default function DiscountsPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [isUpdateDialogOpen, setIsUpdateDialogOpen] = useState(false);
  const [selectedDiscount, setSelectedDiscount] = useState<Discount | null>(
    null
  );
  const [createForm, setCreateForm] = useState<DiscountCreateDto>({
    code: "",
    name: "",
    description: "",
    discountType: 0,
    discountValue: 0,
    minimumOrderAmount: 0,
    maximumDiscountAmount: 0,
    startDate: "",
    endDate: "",
    usageLimit: 0,
    isActive: true,
  });
  const [updateForm, setUpdateForm] = useState<DiscountUpdateDto>({
    code: "",
    name: "",
    description: "",
    discountType: 0,
    discountValue: 0,
    minimumOrderAmount: 0,
    maximumDiscountAmount: 0,
    startDate: "",
    endDate: "",
    usageLimit: 0,
    isActive: true,
  });

  // RTK Query hooks
  const { data: discounts = [], isLoading, error } = useGetAllDiscountsQuery();
  const [createDiscount, { isLoading: isCreating }] =
    useCreateDiscountMutation();
  const [updateDiscount, { isLoading: isUpdating }] =
    useUpdateDiscountMutation();
  const [deleteDiscount, { isLoading: isDeleting }] =
    useDeleteDiscountMutation();

  const handleCreate = async () => {
    try {
      await createDiscount(createForm).unwrap();
      toast.success("Discount created successfully");
      setIsCreateDialogOpen(false);
      setCreateForm({
        code: "",
        name: "",
        description: "",
        discountType: 0,
        discountValue: 0,
        minimumOrderAmount: 0,
        maximumDiscountAmount: 0,
        startDate: "",
        endDate: "",
        usageLimit: 0,
        isActive: true,
      });
    } catch (error) {
      toast.error("Failed to create discount");
      console.error("Error creating discount:", error);
    }
  };

  const handleUpdate = async () => {
    if (!selectedDiscount) return;

    try {
      await updateDiscount({
        id: selectedDiscount.id,
        discount: updateForm,
      }).unwrap();
      toast.success("Discount updated successfully");
      setIsUpdateDialogOpen(false);
      setSelectedDiscount(null);
      setUpdateForm({
        code: "",
        name: "",
        description: "",
        discountType: 0,
        discountValue: 0,
        minimumOrderAmount: 0,
        maximumDiscountAmount: 0,
        startDate: "",
        endDate: "",
        usageLimit: 0,
        isActive: true,
      });
    } catch (error) {
      toast.error("Failed to update discount");
      console.error("Error updating discount:", error);
    }
  };

  const handleDelete = async (id: number) => {
    try {
      await deleteDiscount(id).unwrap();
      toast.success("Discount deleted successfully");
    } catch (error) {
      toast.error("Failed to delete discount");
      console.error("Error deleting discount:", error);
    }
  };

  const openUpdateDialog = (discount: Discount) => {
    setSelectedDiscount(discount);
    setUpdateForm({
      code: discount.code,
      name: discount.name,
      description: discount.description,
      discountType: discount.discountType,
      discountValue: discount.discountValue,
      minimumOrderAmount: discount.minimumOrderAmount,
      maximumDiscountAmount: discount.maximumDiscountAmount,
      startDate: discount.startDate,
      endDate: discount.endDate,
      usageLimit: discount.usageLimit,
      isActive: discount.isActive,
    });
    setIsUpdateDialogOpen(true);
  };

  const getDiscountTypeLabel = (type: number) => {
    return type === 0 ? "Percentage" : "Fixed Amount";
  };

  const getStatusBadge = (discount: Discount) => {
    const now = new Date();
    const startDate = new Date(discount.startDate);
    const endDate = new Date(discount.endDate);
    const isExpired = now > endDate;
    const isNotStarted = now < startDate;
    const isUsageExceeded = discount.usedCount >= discount.usageLimit;

    if (!discount.isActive) return <Badge variant="secondary">Inactive</Badge>;
    if (isExpired) return <Badge variant="destructive">Expired</Badge>;
    if (isNotStarted) return <Badge variant="outline">Not Started</Badge>;
    if (isUsageExceeded)
      return <Badge variant="destructive">Usage Exceeded</Badge>;
    return <Badge variant="default">Active</Badge>;
  };

  const filteredDiscounts = Array.isArray(discounts)
    ? discounts.filter(
        (discount) =>
          discount.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
          discount.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          discount.description.toLowerCase().includes(searchTerm.toLowerCase())
      )
    : [];

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
        <div className="text-red-500">Failed to load discounts</div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Discounts Management</h1>
        <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="h-4 w-4 mr-2" />
              Add Discount
            </Button>
          </DialogTrigger>
          <DialogContent className="max-w-2xl">
            <DialogHeader>
              <DialogTitle>Create New Discount</DialogTitle>
              <DialogDescription>
                Add a new discount code to the system.
              </DialogDescription>
            </DialogHeader>
            <div className="grid grid-cols-2 gap-4">
              <div>
                <Label htmlFor="code">Code</Label>
                <Input
                  id="code"
                  value={createForm.code}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, code: e.target.value })
                  }
                  placeholder="DISCOUNT10"
                />
              </div>
              <div>
                <Label htmlFor="name">Name</Label>
                <Input
                  id="name"
                  value={createForm.name}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, name: e.target.value })
                  }
                  placeholder="Discount name"
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
                  placeholder="Discount description"
                />
              </div>
              <div>
                <Label htmlFor="discountType">Discount Type</Label>
                <select
                  id="discountType"
                  value={createForm.discountType}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      discountType: parseInt(e.target.value),
                    })
                  }
                  className="w-full p-2 border rounded-md"
                >
                  <option value={0}>Percentage</option>
                  <option value={1}>Fixed Amount</option>
                </select>
              </div>
              <div>
                <Label htmlFor="discountValue">Discount Value</Label>
                <Input
                  id="discountValue"
                  type="number"
                  value={createForm.discountValue}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      discountValue: parseFloat(e.target.value),
                    })
                  }
                  placeholder="10"
                />
              </div>
              <div>
                <Label htmlFor="minimumOrderAmount">Minimum Order Amount</Label>
                <Input
                  id="minimumOrderAmount"
                  type="number"
                  value={createForm.minimumOrderAmount}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      minimumOrderAmount: parseFloat(e.target.value),
                    })
                  }
                  placeholder="100"
                />
              </div>
              <div>
                <Label htmlFor="maximumDiscountAmount">
                  Maximum Discount Amount
                </Label>
                <Input
                  id="maximumDiscountAmount"
                  type="number"
                  value={createForm.maximumDiscountAmount}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      maximumDiscountAmount: parseFloat(e.target.value),
                    })
                  }
                  placeholder="50"
                />
              </div>
              <div>
                <Label htmlFor="startDate">Start Date</Label>
                <Input
                  id="startDate"
                  type="datetime-local"
                  value={createForm.startDate}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, startDate: e.target.value })
                  }
                />
              </div>
              <div>
                <Label htmlFor="endDate">End Date</Label>
                <Input
                  id="endDate"
                  type="datetime-local"
                  value={createForm.endDate}
                  onChange={(e) =>
                    setCreateForm({ ...createForm, endDate: e.target.value })
                  }
                />
              </div>
              <div>
                <Label htmlFor="usageLimit">Usage Limit</Label>
                <Input
                  id="usageLimit"
                  type="number"
                  value={createForm.usageLimit}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      usageLimit: parseInt(e.target.value),
                    })
                  }
                  placeholder="100"
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
            placeholder="Search discounts..."
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
              <TableHead>Code</TableHead>
              <TableHead>Name</TableHead>
              <TableHead>Type</TableHead>
              <TableHead>Value</TableHead>
              <TableHead>Usage</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {filteredDiscounts.map((discount) => (
              <TableRow key={discount.id}>
                <TableCell className="font-mono font-medium">
                  {discount.code}
                </TableCell>
                <TableCell>{discount.name}</TableCell>
                <TableCell>
                  {getDiscountTypeLabel(discount.discountType)}
                </TableCell>
                <TableCell>
                  {discount.discountType === 0
                    ? `${discount.discountValue}%`
                    : `$${discount.discountValue}`}
                </TableCell>
                <TableCell>
                  {discount.usedCount}/{discount.usageLimit}
                </TableCell>
                <TableCell>{getStatusBadge(discount)}</TableCell>
                <TableCell>
                  <div className="flex items-center space-x-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => openUpdateDialog(discount)}
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
                          <AlertDialogTitle>Delete Discount</AlertDialogTitle>
                          <AlertDialogDescription>
                            Are you sure you want to delete "{discount.name}"?
                            This action cannot be undone.
                          </AlertDialogDescription>
                        </AlertDialogHeader>
                        <AlertDialogFooter>
                          <AlertDialogCancel>Cancel</AlertDialogCancel>
                          <AlertDialogAction
                            onClick={() => handleDelete(discount.id)}
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
            <DialogTitle>Update Discount</DialogTitle>
            <DialogDescription>
              Update the discount information.
            </DialogDescription>
          </DialogHeader>
          <div className="grid grid-cols-2 gap-4">
            <div>
              <Label htmlFor="update-code">Code</Label>
              <Input
                id="update-code"
                value={updateForm.code}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, code: e.target.value })
                }
                placeholder="DISCOUNT10"
              />
            </div>
            <div>
              <Label htmlFor="update-name">Name</Label>
              <Input
                id="update-name"
                value={updateForm.name}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, name: e.target.value })
                }
                placeholder="Discount name"
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
                placeholder="Discount description"
              />
            </div>
            <div>
              <Label htmlFor="update-discountType">Discount Type</Label>
              <select
                id="update-discountType"
                value={updateForm.discountType}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    discountType: parseInt(e.target.value),
                  })
                }
                className="w-full p-2 border rounded-md"
              >
                <option value={0}>Percentage</option>
                <option value={1}>Fixed Amount</option>
              </select>
            </div>
            <div>
              <Label htmlFor="update-discountValue">Discount Value</Label>
              <Input
                id="update-discountValue"
                type="number"
                value={updateForm.discountValue}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    discountValue: parseFloat(e.target.value),
                  })
                }
                placeholder="10"
              />
            </div>
            <div>
              <Label htmlFor="update-minimumOrderAmount">
                Minimum Order Amount
              </Label>
              <Input
                id="update-minimumOrderAmount"
                type="number"
                value={updateForm.minimumOrderAmount}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    minimumOrderAmount: parseFloat(e.target.value),
                  })
                }
                placeholder="100"
              />
            </div>
            <div>
              <Label htmlFor="update-maximumDiscountAmount">
                Maximum Discount Amount
              </Label>
              <Input
                id="update-maximumDiscountAmount"
                type="number"
                value={updateForm.maximumDiscountAmount}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    maximumDiscountAmount: parseFloat(e.target.value),
                  })
                }
                placeholder="50"
              />
            </div>
            <div>
              <Label htmlFor="update-startDate">Start Date</Label>
              <Input
                id="update-startDate"
                type="datetime-local"
                value={updateForm.startDate}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, startDate: e.target.value })
                }
              />
            </div>
            <div>
              <Label htmlFor="update-endDate">End Date</Label>
              <Input
                id="update-endDate"
                type="datetime-local"
                value={updateForm.endDate}
                onChange={(e) =>
                  setUpdateForm({ ...updateForm, endDate: e.target.value })
                }
              />
            </div>
            <div>
              <Label htmlFor="update-usageLimit">Usage Limit</Label>
              <Input
                id="update-usageLimit"
                type="number"
                value={updateForm.usageLimit}
                onChange={(e) =>
                  setUpdateForm({
                    ...updateForm,
                    usageLimit: parseInt(e.target.value),
                  })
                }
                placeholder="100"
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
