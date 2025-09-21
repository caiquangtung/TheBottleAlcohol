"use client";

import React, { useState } from "react";
import { ImportOrderList } from "@/components/importOrder";
import { ImportOrder } from "@/lib/types/importOrder";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { TrendingUp } from "lucide-react";
import { toast } from "sonner";

export default function ImportOrdersPage() {
  const handleCreateImportOrder = () => {
    // TODO: Open import order creation form
    toast.success("Import Order creation form will be available soon");
  };

  const handleEditImportOrder = (importOrder: ImportOrder) => {
    // TODO: Open import order edit form
    toast.success("Import Order editing will be available soon");
    console.log("Edit import order:", importOrder);
  };

  const handleViewImportOrder = (importOrder: ImportOrder) => {
    // TODO: Open import order detail view
    console.log("View import order:", importOrder);
  };

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold">Import Order Management</h1>
          <p className="text-muted-foreground">
            Manage import orders to add stock and calculate average costs
            automatically
          </p>
        </div>
      </div>

      {/* Workflow Guide */}
      <Card className="border-green-200 bg-green-50 dark:bg-green-950/20">
        <CardContent className="p-4">
          <div className="flex items-start gap-3">
            <TrendingUp className="h-5 w-5 text-green-600 mt-0.5" />
            <div className="space-y-2">
              <p className="text-sm font-medium text-green-800 dark:text-green-200">
                <TrendingUp className="h-3 w-3 inline mr-1" />
                Import Order Workflow for Proper Inventory Management
              </p>
              <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                <div className="space-y-1">
                  <p className="text-xs font-semibold text-green-800 dark:text-green-200">
                    CREATE & APPROVE
                  </p>
                  <ul className="text-xs text-green-700 dark:text-green-300 space-y-1">
                    <li>• Create import order with products & prices</li>
                    <li>• Manager/Admin approves the order</li>
                  </ul>
                </div>
                <div className="space-y-1">
                  <p className="text-xs font-semibold text-green-800 dark:text-green-200">
                    COMPLETE ORDER
                  </p>
                  <ul className="text-xs text-green-700 dark:text-green-300 space-y-1">
                    <li>• System auto-calculates AverageCost</li>
                    <li>• Updates inventory quantities</li>
                    <li>• Syncs Product.StockQuantity</li>
                  </ul>
                </div>
                <div className="space-y-1">
                  <p className="text-xs font-semibold text-green-800 dark:text-green-200">
                    RESULT
                  </p>
                  <ul className="text-xs text-green-700 dark:text-green-300 space-y-1">
                    <li>• Accurate inventory costing</li>
                    <li>• Proper audit trail</li>
                    <li>• Synchronized stock levels</li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Import Order List */}
      <ImportOrderList
        onCreate={handleCreateImportOrder}
        onEdit={handleEditImportOrder}
        onView={handleViewImportOrder}
      />
    </div>
  );
}
