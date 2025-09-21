"use client";

import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import {
  InventoryList,
  InventoryForm,
  StockAdjustmentDialog,
  InventoryTransactionList,
} from "@/components/inventory";
import { useRecalculateAllTotalValuesMutation } from "@/lib/services/inventoryService";
import { Inventory } from "@/lib/types/inventory";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Package, History, RefreshCw, AlertTriangle } from "lucide-react";
import { toast } from "sonner";

export default function InventoryPage() {
  const router = useRouter();
  const [activeTab, setActiveTab] = useState("inventory");
  const [isFormOpen, setIsFormOpen] = useState(false);
  const [isStockAdjustOpen, setIsStockAdjustOpen] = useState(false);
  const [selectedInventory, setSelectedInventory] = useState<Inventory | null>(
    null
  );

  const [recalculateValues, { isLoading: isRecalculating }] =
    useRecalculateAllTotalValuesMutation();

  const handleCreateInventory = () => {
    setSelectedInventory(null);
    setIsFormOpen(true);
  };

  const handleEditInventory = (inventory: Inventory) => {
    setSelectedInventory(inventory);
    setIsFormOpen(true);
  };

  const handleStockAdjust = (inventory: Inventory) => {
    setSelectedInventory(inventory);
    setIsStockAdjustOpen(true);
  };

  const handleViewInventory = (inventory: Inventory) => {
    // Could open a detailed view modal
    console.log("View inventory:", inventory);
  };

  const handleRecalculateValues = async () => {
    try {
      await recalculateValues().unwrap();
      toast.success("All inventory values recalculated successfully");
    } catch {
      toast.error("Failed to recalculate inventory values");
    }
  };

  const handleFormSuccess = () => {
    setIsFormOpen(false);
    setSelectedInventory(null);
  };

  const handleStockAdjustSuccess = () => {
    setIsStockAdjustOpen(false);
    setSelectedInventory(null);
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold">Inventory Management</h1>
          <p className="text-muted-foreground">
            Manage your product inventory, stock levels, and transactions
          </p>
        </div>

        <div className="flex gap-2">
          <Button
            variant="outline"
            onClick={handleRecalculateValues}
            disabled={isRecalculating}
            className="flex items-center gap-2"
          >
            <RefreshCw
              className={`h-4 w-4 ${isRecalculating ? "animate-spin" : ""}`}
            />
            Recalculate Values
          </Button>
        </div>
      </div>

      {/* Workflow Warning */}
      <Card className="border-blue-200 bg-blue-50 dark:bg-blue-950/20">
        <CardContent className="p-4">
          <div className="flex items-start gap-3">
            <AlertTriangle className="h-5 w-5 text-blue-600 mt-0.5" />
            <div className="space-y-2">
              <p className="text-sm font-medium text-blue-800 dark:text-blue-200">
                <Package className="h-3 w-3 inline mr-1" />
                Proper Inventory Management Workflow
              </p>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div>
                  <p className="text-xs font-semibold text-blue-800 dark:text-blue-200 mb-1">
                    CORRECT WORKFLOW:
                  </p>
                  <ol className="text-xs text-blue-700 dark:text-blue-300 space-y-1 list-decimal list-inside">
                    <li>Create Inventory (quantity = 0, averageCost = 0)</li>
                    <li>Create Import Order with products & prices</li>
                    <li>Approve Import Order</li>
                    <li>Complete Import Order → Auto-calculate AverageCost</li>
                    <li>Use Stock Adjustments for corrections only</li>
                  </ol>
                </div>
                <div>
                  <p className="text-xs font-semibold text-orange-800 dark:text-orange-200 mb-1">
                    AVOID:
                  </p>
                  <ul className="text-xs text-orange-700 dark:text-orange-300 space-y-1">
                    <li>• Manual AverageCost input</li>
                    <li>• Direct quantity increases without Import Orders</li>
                    <li>• Bypassing Import Order workflow</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Main Content */}
      <Tabs value={activeTab} onValueChange={setActiveTab}>
        <TabsList className="grid w-full grid-cols-2">
          <TabsTrigger value="inventory" className="flex items-center gap-2">
            <Package className="h-4 w-4" />
            Inventory
          </TabsTrigger>
          <TabsTrigger value="transactions" className="flex items-center gap-2">
            <History className="h-4 w-4" />
            Transactions
          </TabsTrigger>
        </TabsList>

        <TabsContent value="inventory" className="space-y-6">
          {/* Import Order Integration Hint */}
          <Card className="border-green-200 bg-green-50 dark:bg-green-950/20">
            <CardContent className="p-4">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <Package className="h-5 w-5 text-green-600" />
                  <div>
                    <p className="text-sm font-medium text-green-800 dark:text-green-200">
                      <Package className="h-3 w-3 inline mr-1" />
                      Need to add stock with proper costing?
                    </p>
                    <p className="text-xs text-green-700 dark:text-green-300">
                      Use Import Orders to add stock and automatically calculate
                      Average Cost
                    </p>
                  </div>
                </div>
                <Button
                  variant="outline"
                  size="sm"
                  className="border-green-300 text-green-700 hover:bg-green-100"
                  onClick={() => {
                    router.push("/admin/import-orders");
                  }}
                >
                  Go to Import Orders
                </Button>
              </div>
            </CardContent>
          </Card>

          <InventoryList
            onCreate={handleCreateInventory}
            onEdit={handleEditInventory}
            onView={handleViewInventory}
            onStockAdjust={handleStockAdjust}
          />
        </TabsContent>

        <TabsContent value="transactions" className="space-y-6">
          {/* Transaction Guide */}
          <Card className="border-purple-200 bg-purple-50 dark:bg-purple-950/20">
            <CardContent className="p-4">
              <div className="flex items-start gap-3">
                <History className="h-5 w-5 text-purple-600 mt-0.5" />
                <div className="space-y-2">
                  <p className="text-sm font-medium text-purple-800 dark:text-purple-200">
                    <History className="h-3 w-3 inline mr-1" />
                    Inventory Transaction Types
                  </p>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <p className="text-xs font-semibold text-purple-800 dark:text-purple-200 mb-1">
                        AUTOMATIC TRANSACTIONS:
                      </p>
                      <ul className="text-xs text-purple-700 dark:text-purple-300 space-y-1">
                        <li>
                          • <strong>Import</strong> - From completed Import
                          Orders
                        </li>
                        <li>
                          • <strong>Export</strong> - From customer orders
                        </li>
                      </ul>
                    </div>
                    <div>
                      <p className="text-xs font-semibold text-purple-800 dark:text-purple-200 mb-1">
                        MANUAL TRANSACTIONS:
                      </p>
                      <ul className="text-xs text-purple-700 dark:text-purple-300 space-y-1">
                        <li>
                          • <strong>Adjustment</strong> - Stock corrections
                          (damage, theft)
                        </li>
                        <li>
                          • <strong>Transfer</strong> - Between locations
                        </li>
                      </ul>
                    </div>
                  </div>
                  <div className="flex gap-2 mt-3">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => router.push("/admin/import-orders")}
                      className="border-purple-300 text-purple-700 hover:bg-purple-100"
                    >
                      View Import Orders
                    </Button>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => router.push("/admin/orders")}
                      className="border-purple-300 text-purple-700 hover:bg-purple-100"
                    >
                      View Customer Orders
                    </Button>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          <InventoryTransactionList showProductColumn={true} />
        </TabsContent>
      </Tabs>

      {/* Dialogs */}
      <InventoryForm
        open={isFormOpen}
        onClose={() => setIsFormOpen(false)}
        inventory={selectedInventory}
        onSuccess={handleFormSuccess}
      />

      <StockAdjustmentDialog
        open={isStockAdjustOpen}
        onClose={() => setIsStockAdjustOpen(false)}
        inventory={selectedInventory}
        onSuccess={handleStockAdjustSuccess}
      />
    </div>
  );
}
