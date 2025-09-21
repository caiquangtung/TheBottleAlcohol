"use client";

import React, { useState, useMemo } from "react";
import {
  useGetAllInventoryTransactionsQuery,
  useDeleteInventoryTransactionMutation,
} from "@/lib/services/inventoryService";
import {
  InventoryTransaction,
  InventoryTransactionFilterDto,
  InventoryTransactionType,
  InventoryTransactionStatus,
  ReferenceType,
} from "@/lib/types/inventory";
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
  History,
  TrendingUp,
  TrendingDown,
  Edit,
  Trash2,
  Plus,
  ArrowUpDown,
  Package,
  FileText,
} from "lucide-react";
import { toast } from "sonner";

interface InventoryTransactionListProps {
  productId?: number;
  onEdit?: (transaction: InventoryTransaction) => void;
  onCreate?: () => void;
  showProductColumn?: boolean;
}

export function InventoryTransactionList({
  productId,
  onEdit,
  onCreate,
  showProductColumn = true,
}: InventoryTransactionListProps) {
  const [filter, setFilter] = useState<InventoryTransactionFilterDto>({
    productId,
    page: 1,
    pageSize: 20,
    sortBy: "transactionDate",
    sortOrder: "desc",
  });

  const [searchTerm, setSearchTerm] = useState("");

  const {
    data: transactionData,
    isLoading,
    error,
  } = useGetAllInventoryTransactionsQuery(filter);

  const [deleteTransaction] = useDeleteInventoryTransactionMutation();

  // Filter transactions based on search term
  const filteredTransactions = useMemo(() => {
    if (!transactionData?.items || !searchTerm) {
      return transactionData?.items || [];
    }

    return transactionData.items.filter(
      (item) =>
        item.transactionNumber
          .toLowerCase()
          .includes(searchTerm.toLowerCase()) ||
        item.productName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        item.notes?.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [transactionData?.items, searchTerm]);

  const handleDelete = async (id: number) => {
    try {
      await deleteTransaction(id).unwrap();
      toast.success("Transaction deleted successfully");
    } catch (error) {
      toast.error("Failed to delete transaction");
    }
  };

  const handleFilterChange = (
    key: keyof InventoryTransactionFilterDto,
    value: any
  ) => {
    setFilter((prev) => ({ ...prev, [key]: value, page: 1 }));
  };

  const getTransactionTypeIcon = (type: InventoryTransactionType) => {
    switch (type) {
      case InventoryTransactionType.Import:
        return TrendingUp;
      case InventoryTransactionType.Export:
        return TrendingDown;
      case InventoryTransactionType.Adjustment:
        return ArrowUpDown;
      case InventoryTransactionType.Transfer:
        return Package;
      case InventoryTransactionType.Return:
        return TrendingDown;
      default:
        return History;
    }
  };

  const getTransactionTypeColor = (type: InventoryTransactionType) => {
    switch (type) {
      case InventoryTransactionType.Import:
        return "text-green-600";
      case InventoryTransactionType.Export:
        return "text-red-600";
      case InventoryTransactionType.Adjustment:
        return "text-blue-600";
      case InventoryTransactionType.Transfer:
        return "text-purple-600";
      case InventoryTransactionType.Return:
        return "text-orange-600";
      default:
        return "text-gray-600";
    }
  };

  const getStatusVariant = (status: InventoryTransactionStatus) => {
    switch (status) {
      case InventoryTransactionStatus.Pending:
        return "secondary" as const;
      case InventoryTransactionStatus.Completed:
        return "default" as const;
      case InventoryTransactionStatus.Cancelled:
        return "destructive" as const;
      default:
        return "secondary" as const;
    }
  };

  const getReferenceTypeLabel = (type: ReferenceType) => {
    switch (type) {
      case ReferenceType.ImportOrder:
        return "Import Order";
      case ReferenceType.Order:
        return "Order";
      case ReferenceType.Manual:
        return "Manual";
      default:
        return type;
    }
  };

  if (error) {
    return (
      <Card>
        <CardContent className="p-6">
          <div className="text-center text-red-500">
            Error loading transaction data
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <Card>
        <CardHeader>
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <div>
              <CardTitle className="flex items-center gap-2">
                <History className="h-5 w-5" />
                Inventory Transactions
              </CardTitle>
              {productId && (
                <p className="text-sm text-muted-foreground mt-1">
                  Showing transactions for selected product
                </p>
              )}
            </div>
            {onCreate && (
              <Button onClick={onCreate} className="flex items-center gap-2">
                <Plus className="h-4 w-4" />
                Add Transaction
              </Button>
            )}
          </div>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col sm:flex-row gap-4 mb-6">
            <SearchInput
              value={searchTerm}
              onChange={setSearchTerm}
              placeholder="Search by transaction number, product, or notes..."
              className="flex-1"
            />

            <div className="flex gap-2">
              <Select
                value={filter.transactionType || "all"}
                onValueChange={(value) =>
                  handleFilterChange(
                    "transactionType",
                    value === "all" ? undefined : value
                  )
                }
              >
                <SelectTrigger className="w-40">
                  <SelectValue placeholder="All Types" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Types</SelectItem>
                  <SelectItem value={InventoryTransactionType.Import}>
                    Import
                  </SelectItem>
                  <SelectItem value={InventoryTransactionType.Export}>
                    Export
                  </SelectItem>
                  <SelectItem value={InventoryTransactionType.Adjustment}>
                    Adjustment
                  </SelectItem>
                  <SelectItem value={InventoryTransactionType.Transfer}>
                    Transfer
                  </SelectItem>
                  <SelectItem value={InventoryTransactionType.Return}>
                    Return to Supplier
                  </SelectItem>
                </SelectContent>
              </Select>

              <Select
                value={filter.status || "all"}
                onValueChange={(value) =>
                  handleFilterChange(
                    "status",
                    value === "all" ? undefined : value
                  )
                }
              >
                <SelectTrigger className="w-32">
                  <SelectValue placeholder="All Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status</SelectItem>
                  <SelectItem value={InventoryTransactionStatus.Pending}>
                    Pending
                  </SelectItem>
                  <SelectItem value={InventoryTransactionStatus.Completed}>
                    Completed
                  </SelectItem>
                  <SelectItem value={InventoryTransactionStatus.Cancelled}>
                    Cancelled
                  </SelectItem>
                </SelectContent>
              </Select>

              <Select
                value={filter.sortBy}
                onValueChange={(value) => handleFilterChange("sortBy", value)}
              >
                <SelectTrigger className="w-40">
                  <SelectValue placeholder="Sort by" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="transactionDate">Date</SelectItem>
                  <SelectItem value="transactionNumber">
                    Transaction #
                  </SelectItem>
                  <SelectItem value="transactionType">Type</SelectItem>
                  <SelectItem value="status">Status</SelectItem>
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
                  <SelectItem value="desc">Newest First</SelectItem>
                  <SelectItem value="asc">Oldest First</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Transaction Table */}
          <div className="border rounded-lg">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Transaction #</TableHead>
                  {showProductColumn && <TableHead>Product</TableHead>}
                  <TableHead>Type</TableHead>
                  <TableHead>Quantity</TableHead>
                  <TableHead>Reference</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Date</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {isLoading ? (
                  <TableRow>
                    <TableCell
                      colSpan={showProductColumn ? 8 : 7}
                      className="text-center py-8"
                    >
                      Loading...
                    </TableCell>
                  </TableRow>
                ) : filteredTransactions.length === 0 ? (
                  <TableRow>
                    <TableCell
                      colSpan={showProductColumn ? 8 : 7}
                      className="text-center py-8"
                    >
                      No transactions found
                    </TableCell>
                  </TableRow>
                ) : (
                  filteredTransactions.map((transaction) => {
                    const TypeIcon = getTransactionTypeIcon(
                      transaction.transactionType
                    );
                    const typeColor = getTransactionTypeColor(
                      transaction.transactionType
                    );

                    return (
                      <TableRow key={transaction.id}>
                        <TableCell className="font-mono text-sm">
                          {transaction.transactionNumber}
                        </TableCell>
                        {showProductColumn && (
                          <TableCell className="font-medium">
                            {transaction.productName ||
                              `Product #${transaction.productId}`}
                          </TableCell>
                        )}
                        <TableCell>
                          <div
                            className={`flex items-center gap-2 ${typeColor}`}
                          >
                            <TypeIcon className="h-4 w-4" />
                            {transaction.transactionType}
                          </div>
                        </TableCell>
                        <TableCell>
                          <span
                            className={`font-medium ${
                              transaction.transactionType ===
                              InventoryTransactionType.Import
                                ? "text-green-600"
                                : transaction.transactionType ===
                                  InventoryTransactionType.Export
                                ? "text-red-600"
                                : "text-blue-600"
                            }`}
                          >
                            {transaction.transactionType ===
                            InventoryTransactionType.Import
                              ? "+"
                              : transaction.transactionType ===
                                InventoryTransactionType.Export
                              ? "-"
                              : "±"}
                            {Math.abs(transaction.quantity)}
                          </span>
                        </TableCell>
                        <TableCell>
                          <div className="text-sm">
                            <div className="font-medium">
                              {getReferenceTypeLabel(transaction.referenceType)}
                            </div>
                            <div className="text-muted-foreground">
                              #{transaction.referenceId}
                            </div>
                          </div>
                        </TableCell>
                        <TableCell>
                          <Badge variant={getStatusVariant(transaction.status)}>
                            {transaction.status}
                          </Badge>
                        </TableCell>
                        <TableCell>
                          <div className="text-sm">
                            <div className="font-medium">
                              {new Date(
                                transaction.transactionDate
                              ).toLocaleDateString()}
                            </div>
                            <div className="text-muted-foreground">
                              {new Date(
                                transaction.transactionDate
                              ).toLocaleTimeString()}
                            </div>
                          </div>
                        </TableCell>
                        <TableCell className="text-right">
                          <div className="flex items-center justify-end gap-2">
                            {transaction.notes && (
                              <div className="group relative">
                                <FileText className="h-4 w-4 text-muted-foreground cursor-help" />
                                <div className="absolute bottom-full right-0 mb-2 hidden group-hover:block">
                                  <div className="bg-black text-white text-xs rounded px-2 py-1 whitespace-nowrap max-w-xs">
                                    {transaction.notes}
                                  </div>
                                </div>
                              </div>
                            )}
                            {onEdit && (
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => onEdit(transaction)}
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
                                    Delete Transaction
                                  </AlertDialogTitle>
                                  <AlertDialogDescription>
                                    Are you sure you want to delete transaction
                                    "{transaction.transactionNumber}"? This
                                    action cannot be undone.
                                  </AlertDialogDescription>
                                </AlertDialogHeader>
                                <AlertDialogFooter>
                                  <AlertDialogCancel>Cancel</AlertDialogCancel>
                                  <AlertDialogAction
                                    onClick={() => handleDelete(transaction.id)}
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
          {transactionData && transactionData.totalPages > 1 && (
            <div className="flex justify-between items-center mt-4">
              <div className="text-sm text-muted-foreground">
                Showing {((filter.page || 1) - 1) * (filter.pageSize || 20) + 1}{" "}
                to{" "}
                {Math.min(
                  (filter.page || 1) * (filter.pageSize || 20),
                  transactionData.totalItems
                )}{" "}
                of {transactionData.totalItems} transactions
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
                  disabled={(filter.page || 1) >= transactionData.totalPages}
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

export default InventoryTransactionList;
