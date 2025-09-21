export interface ProfitCalculationInput {
  averageCost: number;
  sellingPrice?: number;
  targetMarginPercentage?: number;
  currentStock?: number;
}

export interface ProfitAnalysisResult {
  averageCost: number;
  sellingPrice: number;
  profitPerUnit: number;
  profitMarginPercentage: number;
  targetMarginPercentage: number;
  recommendedSellingPrice: number;
  isMarginAchieved: boolean;
  currentStock: number;
  potentialTotalProfit: number;
}

export class ProfitCalculator {
  /**
   * Calculate profit per unit
   */
  static calculateProfitPerUnit(
    sellingPrice: number,
    averageCost: number
  ): number {
    return sellingPrice - averageCost;
  }

  /**
   * Calculate profit margin percentage
   */
  static calculateMarginPercentage(
    sellingPrice: number,
    averageCost: number
  ): number {
    if (sellingPrice <= 0) return 0;
    const profit = this.calculateProfitPerUnit(sellingPrice, averageCost);
    return (profit / sellingPrice) * 100;
  }

  /**
   * Calculate recommended selling price for target margin
   */
  static calculateRecommendedPrice(
    averageCost: number,
    targetMarginPercentage: number
  ): number {
    if (targetMarginPercentage >= 100 || targetMarginPercentage < 0) {
      throw new Error("Target margin must be between 0 and 100");
    }
    return averageCost / (1 - targetMarginPercentage / 100);
  }

  /**
   * Calculate total profit for given quantity
   */
  static calculateTotalProfit(profitPerUnit: number, quantity: number): number {
    return profitPerUnit * quantity;
  }

  /**
   * Calculate markup percentage (cost-based)
   */
  static calculateMarkupPercentage(
    sellingPrice: number,
    averageCost: number
  ): number {
    if (averageCost <= 0) return 0;
    const profit = this.calculateProfitPerUnit(sellingPrice, averageCost);
    return (profit / averageCost) * 100;
  }

  /**
   * Calculate break-even price (minimum price to avoid loss)
   */
  static calculateBreakEvenPrice(
    averageCost: number,
    additionalCosts: number = 0
  ): number {
    return averageCost + additionalCosts;
  }

  /**
   * Comprehensive profit analysis
   */
  static analyzeProduct(input: ProfitCalculationInput): ProfitAnalysisResult {
    const {
      averageCost,
      sellingPrice = 0,
      targetMarginPercentage = 30,
      currentStock = 0,
    } = input;

    if (averageCost < 0) {
      throw new Error("Average cost cannot be negative");
    }

    const profitPerUnit = this.calculateProfitPerUnit(
      sellingPrice,
      averageCost
    );
    const marginPercentage = this.calculateMarginPercentage(
      sellingPrice,
      averageCost
    );
    const recommendedPrice =
      averageCost > 0
        ? this.calculateRecommendedPrice(averageCost, targetMarginPercentage)
        : sellingPrice;
    const totalProfit = this.calculateTotalProfit(profitPerUnit, currentStock);

    return {
      averageCost,
      sellingPrice,
      profitPerUnit,
      profitMarginPercentage: Math.round(marginPercentage * 100) / 100,
      targetMarginPercentage,
      recommendedSellingPrice: Math.round(recommendedPrice),
      isMarginAchieved: marginPercentage >= targetMarginPercentage,
      currentStock,
      potentialTotalProfit: totalProfit,
    };
  }

  /**
   * Compare two pricing scenarios
   */
  static comparePricingScenarios(
    averageCost: number,
    scenario1Price: number,
    scenario2Price: number,
    currentStock: number = 0
  ) {
    const scenario1 = this.analyzeProduct({
      averageCost,
      sellingPrice: scenario1Price,
      currentStock,
    });

    const scenario2 = this.analyzeProduct({
      averageCost,
      sellingPrice: scenario2Price,
      currentStock,
    });

    return {
      scenario1,
      scenario2,
      comparison: {
        profitDifference: scenario2.profitPerUnit - scenario1.profitPerUnit,
        marginDifference:
          scenario2.profitMarginPercentage - scenario1.profitMarginPercentage,
        totalProfitDifference:
          scenario2.potentialTotalProfit - scenario1.potentialTotalProfit,
        recommendation:
          scenario2.profitMarginPercentage > scenario1.profitMarginPercentage
            ? "scenario2"
            : "scenario1",
      },
    };
  }

  /**
   * Calculate optimal price for maximum profit within constraints
   */
  static calculateOptimalPrice(
    averageCost: number,
    minMarginPercentage: number,
    maxPrice: number,
    targetMarginPercentage: number = 35
  ): {
    optimalPrice: number;
    achievableMargin: number;
    isTargetAchievable: boolean;
  } {
    const minPrice = this.calculateRecommendedPrice(
      averageCost,
      minMarginPercentage
    );
    const targetPrice = this.calculateRecommendedPrice(
      averageCost,
      targetMarginPercentage
    );

    let optimalPrice: number;
    let achievableMargin: number;
    let isTargetAchievable: boolean;

    if (targetPrice <= maxPrice) {
      // Target margin is achievable
      optimalPrice = targetPrice;
      achievableMargin = targetMarginPercentage;
      isTargetAchievable = true;
    } else if (minPrice <= maxPrice) {
      // Use max price within constraints
      optimalPrice = maxPrice;
      achievableMargin = this.calculateMarginPercentage(maxPrice, averageCost);
      isTargetAchievable = false;
    } else {
      // Even minimum margin not achievable
      optimalPrice = minPrice;
      achievableMargin = minMarginPercentage;
      isTargetAchievable = false;
    }

    return {
      optimalPrice: Math.round(optimalPrice),
      achievableMargin: Math.round(achievableMargin * 100) / 100,
      isTargetAchievable,
    };
  }

  /**
   * Bulk profit analysis for multiple products
   */
  static analyzeBulkProducts(
    products: Array<{
      productId: number;
      productName: string;
      averageCost: number;
      sellingPrice: number;
      currentStock: number;
      targetMarginPercentage?: number;
    }>
  ): {
    totalInventoryValue: number;
    totalPotentialRevenue: number;
    totalPotentialProfit: number;
    averageMarginPercentage: number;
    productsAboveTarget: number;
    productsBelowTarget: number;
    topProfitableProducts: ProfitAnalysisResult[];
    lowMarginProducts: ProfitAnalysisResult[];
  } {
    const analyses = products.map((product) =>
      this.analyzeProduct({
        averageCost: product.averageCost,
        sellingPrice: product.sellingPrice,
        targetMarginPercentage: product.targetMarginPercentage || 30,
        currentStock: product.currentStock,
      })
    );

    const totalInventoryValue = analyses.reduce(
      (sum, a) => sum + a.averageCost * a.currentStock,
      0
    );
    const totalPotentialRevenue = analyses.reduce(
      (sum, a) => sum + a.sellingPrice * a.currentStock,
      0
    );
    const totalPotentialProfit = analyses.reduce(
      (sum, a) => sum + a.potentialTotalProfit,
      0
    );
    const averageMarginPercentage =
      analyses.reduce((sum, a) => sum + a.profitMarginPercentage, 0) /
      analyses.length;

    return {
      totalInventoryValue,
      totalPotentialRevenue,
      totalPotentialProfit,
      averageMarginPercentage: Math.round(averageMarginPercentage * 100) / 100,
      productsAboveTarget: analyses.filter((a) => a.isMarginAchieved).length,
      productsBelowTarget: analyses.filter((a) => !a.isMarginAchieved).length,
      topProfitableProducts: analyses
        .sort((a, b) => b.profitMarginPercentage - a.profitMarginPercentage)
        .slice(0, 10),
      lowMarginProducts: analyses
        .filter((a) => a.profitMarginPercentage < 10)
        .sort((a, b) => a.profitMarginPercentage - b.profitMarginPercentage),
    };
  }
}

// Helper functions cho formatting
export const formatPercentage = (value: number): string => {
  return `${value.toFixed(2)}%`;
};

export const formatCurrency = (value: number): string => {
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency: "VND",
  }).format(value);
};

export const formatProfitStatus = (
  isMarginAchieved: boolean
): {
  label: string;
  color: string;
  iconName: string;
} => {
  return isMarginAchieved
    ? {
        label: "Target Achieved",
        color: "text-green-600",
        iconName: "CheckCircle",
      }
    : {
        label: "Below Target",
        color: "text-orange-600",
        iconName: "AlertTriangle",
      };
};
