"use client";

import React, { useState, useMemo } from "react";
import { Inventory } from "@/lib/types/inventory";
import { useGetProductByIdQuery } from "@/lib/services/productService";
import { ProfitCalculator } from "@/lib/utils/profitCalculator";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import {
  TrendingUp,
  Calculator,
  DollarSign,
  Target,
  CheckCircle,
  AlertTriangle,
  Info,
} from "lucide-react";
import { formatCurrency } from "@/lib/utils";

interface ProfitCalculatorCardProps {
  inventory: Inventory;
  onClose?: () => void;
}

export function ProfitCalculatorCard({
  inventory,
  onClose,
}: ProfitCalculatorCardProps) {
  const [customPrice, setCustomPrice] = useState<number>(0);
  const [targetMargin, setTargetMargin] = useState<number>(35);

  const { data: product } = useGetProductByIdQuery(inventory.productId);

  // Current profit analysis
  const currentAnalysis = useMemo(() => {
    if (!product || inventory.averageCost <= 0) return null;

    return ProfitCalculator.analyzeProduct({
      averageCost: inventory.averageCost,
      sellingPrice: product.price,
      targetMarginPercentage: product.targetMarginPercentage || 30,
      currentStock: inventory.quantity,
    });
  }, [inventory, product]);

  // Custom price analysis
  const customAnalysis = useMemo(() => {
    if (!customPrice || customPrice <= 0 || inventory.averageCost <= 0)
      return null;

    return ProfitCalculator.analyzeProduct({
      averageCost: inventory.averageCost,
      sellingPrice: customPrice,
      targetMarginPercentage: targetMargin,
      currentStock: inventory.quantity,
    });
  }, [inventory.averageCost, customPrice, targetMargin, inventory.quantity]);

  // Recommended price for target margin
  const recommendedPrice = useMemo(() => {
    if (inventory.averageCost <= 0) return 0;
    try {
      return ProfitCalculator.calculateRecommendedPrice(
        inventory.averageCost,
        targetMargin
      );
    } catch {
      return 0;
    }
  }, [inventory.averageCost, targetMargin]);

  if (inventory.averageCost <= 0) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Calculator className="h-5 w-5" />
            Profit Calculator
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex items-center gap-3 p-4 bg-orange-50 dark:bg-orange-950/20 rounded-lg">
            <AlertTriangle className="h-5 w-5 text-orange-600" />
            <div>
              <p className="text-sm font-medium text-orange-800 dark:text-orange-200">
                Cannot calculate profit
              </p>
              <p className="text-xs text-orange-700 dark:text-orange-300">
                AverageCost = 0. Complete Import Orders first to establish
                product costing.
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
    );
  }

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <CardTitle className="flex items-center gap-2">
            <Calculator className="h-5 w-5" />
            Profit Calculator - {inventory.productName}
          </CardTitle>
          {onClose && (
            <Button variant="ghost" size="sm" onClick={onClose}>
              ×
            </Button>
          )}
        </div>
      </CardHeader>
      <CardContent className="space-y-6">
        {/* Current Profit Analysis */}
        {currentAnalysis && (
          <div className="space-y-4">
            <div className="flex items-center gap-2">
              <TrendingUp className="h-4 w-4" />
              <h4 className="font-semibold">Current Pricing Analysis</h4>
            </div>

            <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
              <div className="space-y-1">
                <p className="text-xs text-muted-foreground">Average Cost</p>
                <p className="text-sm font-semibold">
                  {formatCurrency(currentAnalysis.averageCost)}
                </p>
              </div>
              <div className="space-y-1">
                <p className="text-xs text-muted-foreground">Selling Price</p>
                <p className="text-sm font-semibold">
                  {formatCurrency(currentAnalysis.sellingPrice)}
                </p>
              </div>
              <div className="space-y-1">
                <p className="text-xs text-muted-foreground">Profit/Unit</p>
                <p
                  className={`text-sm font-semibold ${
                    currentAnalysis.profitPerUnit >= 0
                      ? "text-green-600"
                      : "text-red-600"
                  }`}
                >
                  {formatCurrency(currentAnalysis.profitPerUnit)}
                </p>
              </div>
              <div className="space-y-1">
                <p className="text-xs text-muted-foreground">Margin</p>
                <div className="flex items-center gap-1">
                  <p
                    className={`text-sm font-semibold ${
                      currentAnalysis.isMarginAchieved
                        ? "text-green-600"
                        : "text-orange-600"
                    }`}
                  >
                    {currentAnalysis.profitMarginPercentage.toFixed(2)}%
                  </p>
                  {currentAnalysis.isMarginAchieved ? (
                    <CheckCircle className="h-3 w-3 text-green-600" />
                  ) : (
                    <AlertTriangle className="h-3 w-3 text-orange-600" />
                  )}
                </div>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-1">
                <p className="text-xs text-muted-foreground">Target Margin</p>
                <p className="text-sm font-semibold">
                  {currentAnalysis.targetMarginPercentage}%
                </p>
              </div>
              <div className="space-y-1">
                <p className="text-xs text-muted-foreground">
                  Recommended Price
                </p>
                <p className="text-sm font-semibold text-blue-600">
                  {formatCurrency(currentAnalysis.recommendedSellingPrice)}
                </p>
              </div>
            </div>

            <div className="bg-muted/50 p-3 rounded-lg">
              <div className="flex justify-between items-center">
                <div>
                  <p className="text-xs text-muted-foreground">
                    Total Profit Potential
                  </p>
                  <p className="text-xs text-muted-foreground">
                    Stock: {currentAnalysis.currentStock} units
                  </p>
                </div>
                <p className="text-lg font-bold text-green-600">
                  {formatCurrency(currentAnalysis.potentialTotalProfit)}
                </p>
              </div>
            </div>
          </div>
        )}

        {/* Manual Calculator */}
        <div className="space-y-4 border-t pt-4">
          <div className="flex items-center gap-2">
            <Target className="h-4 w-4" />
            <h4 className="font-semibold">Manual Profit Calculator</h4>
          </div>

          <div className="grid grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="customPrice" className="text-xs">
                Test Selling Price
              </Label>
              <Input
                id="customPrice"
                type="number"
                min="0"
                step="1000"
                placeholder="Enter test price"
                value={customPrice || ""}
                onChange={(e) =>
                  setCustomPrice(parseFloat(e.target.value) || 0)
                }
              />
            </div>
            <div className="space-y-2">
              <Label htmlFor="targetMargin" className="text-xs">
                Target Margin %
              </Label>
              <Input
                id="targetMargin"
                type="number"
                min="0"
                max="100"
                step="1"
                value={targetMargin}
                onChange={(e) =>
                  setTargetMargin(parseFloat(e.target.value) || 30)
                }
              />
            </div>
          </div>

          {/* Quick Price Buttons */}
          <div className="space-y-2">
            <p className="text-xs text-muted-foreground">Quick Test Prices:</p>
            <div className="flex gap-2 flex-wrap">
              <Button
                variant="outline"
                size="sm"
                onClick={() => setCustomPrice(recommendedPrice)}
              >
                Recommended ({formatCurrency(recommendedPrice)})
              </Button>
              <Button
                variant="outline"
                size="sm"
                onClick={() => setCustomPrice(inventory.averageCost * 1.2)}
              >
                +20% ({formatCurrency(inventory.averageCost * 1.2)})
              </Button>
              <Button
                variant="outline"
                size="sm"
                onClick={() => setCustomPrice(inventory.averageCost * 1.5)}
              >
                +50% ({formatCurrency(inventory.averageCost * 1.5)})
              </Button>
            </div>
          </div>

          {/* Custom Analysis Results */}
          {customAnalysis && (
            <div className="bg-blue-50 dark:bg-blue-950/20 p-4 rounded-lg space-y-3">
              <div className="flex items-center gap-2">
                <Info className="h-4 w-4 text-blue-600" />
                <p className="text-sm font-medium text-blue-800 dark:text-blue-200">
                  Manual Calculation Results
                </p>
              </div>

              <div className="grid grid-cols-2 gap-4 text-sm">
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">
                      Profit per Unit:
                    </span>
                    <span
                      className={`font-semibold ${
                        customAnalysis.profitPerUnit >= 0
                          ? "text-green-600"
                          : "text-red-600"
                      }`}
                    >
                      {formatCurrency(customAnalysis.profitPerUnit)}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Margin:</span>
                    <div className="flex items-center gap-1">
                      <span
                        className={`font-semibold ${
                          customAnalysis.isMarginAchieved
                            ? "text-green-600"
                            : "text-orange-600"
                        }`}
                      >
                        {customAnalysis.profitMarginPercentage.toFixed(2)}%
                      </span>
                      {customAnalysis.isMarginAchieved ? (
                        <CheckCircle className="h-3 w-3 text-green-600" />
                      ) : (
                        <AlertTriangle className="h-3 w-3 text-orange-600" />
                      )}
                    </div>
                  </div>
                </div>

                <div className="space-y-2">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Total Profit:</span>
                    <span className="font-semibold text-green-600">
                      {formatCurrency(customAnalysis.potentialTotalProfit)}
                    </span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">vs Target:</span>
                    <span
                      className={`font-semibold ${
                        customAnalysis.isMarginAchieved
                          ? "text-green-600"
                          : "text-red-600"
                      }`}
                    >
                      {customAnalysis.isMarginAchieved
                        ? "Achieved"
                        : "Below Target"}
                    </span>
                  </div>
                </div>
              </div>

              {/* Comparison với current price */}
              {currentAnalysis && (
                <div className="border-t pt-3 mt-3">
                  <p className="text-xs font-medium text-blue-800 dark:text-blue-200 mb-2">
                    Comparison với Current Price:
                  </p>
                  <div className="grid grid-cols-3 gap-2 text-xs">
                    <div>
                      <span className="text-muted-foreground">
                        Profit Difference:
                      </span>
                      <p
                        className={`font-semibold ${
                          customAnalysis.profitPerUnit -
                            currentAnalysis.profitPerUnit >=
                          0
                            ? "text-green-600"
                            : "text-red-600"
                        }`}
                      >
                        {customAnalysis.profitPerUnit -
                          currentAnalysis.profitPerUnit >=
                        0
                          ? "+"
                          : ""}
                        {formatCurrency(
                          customAnalysis.profitPerUnit -
                            currentAnalysis.profitPerUnit
                        )}
                      </p>
                    </div>
                    <div>
                      <span className="text-muted-foreground">
                        Margin Difference:
                      </span>
                      <p
                        className={`font-semibold ${
                          customAnalysis.profitMarginPercentage -
                            currentAnalysis.profitMarginPercentage >=
                          0
                            ? "text-green-600"
                            : "text-red-600"
                        }`}
                      >
                        {customAnalysis.profitMarginPercentage -
                          currentAnalysis.profitMarginPercentage >=
                        0
                          ? "+"
                          : ""}
                        {(
                          customAnalysis.profitMarginPercentage -
                          currentAnalysis.profitMarginPercentage
                        ).toFixed(2)}
                        %
                      </p>
                    </div>
                    <div>
                      <span className="text-muted-foreground">
                        Total Difference:
                      </span>
                      <p
                        className={`font-semibold ${
                          customAnalysis.potentialTotalProfit -
                            currentAnalysis.potentialTotalProfit >=
                          0
                            ? "text-green-600"
                            : "text-red-600"
                        }`}
                      >
                        {customAnalysis.potentialTotalProfit -
                          currentAnalysis.potentialTotalProfit >=
                        0
                          ? "+"
                          : ""}
                        {formatCurrency(
                          customAnalysis.potentialTotalProfit -
                            currentAnalysis.potentialTotalProfit
                        )}
                      </p>
                    </div>
                  </div>
                </div>
              )}
            </div>
          )}
        </div>

        {/* Formula Reference */}
        <div className="bg-gray-50 dark:bg-gray-900/50 p-4 rounded-lg">
          <div className="flex items-center gap-2 mb-3">
            <DollarSign className="h-4 w-4" />
            <h5 className="font-medium text-sm">Profit Calculation Formulas</h5>
          </div>
          <div className="space-y-2 text-xs">
            <div>
              <strong>Profit per Unit:</strong> Selling Price - Average Cost
            </div>
            <div>
              <strong>Margin %:</strong> (Profit per Unit ÷ Selling Price) × 100
            </div>
            <div>
              <strong>Recommended Price:</strong> Average Cost ÷ (1 - Target
              Margin%)
            </div>
            <div>
              <strong>Total Profit:</strong> Profit per Unit × Current Stock
            </div>
          </div>
        </div>

        {/* Action Buttons */}
        <div className="flex gap-2">
          <Button
            variant="outline"
            size="sm"
            onClick={() => setCustomPrice(recommendedPrice)}
            disabled={recommendedPrice <= 0}
          >
            Use Recommended Price
          </Button>
          <Button
            variant="outline"
            size="sm"
            onClick={() => {
              setCustomPrice(0);
              setTargetMargin(35);
            }}
          >
            Reset Calculator
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}

export default ProfitCalculatorCard;
