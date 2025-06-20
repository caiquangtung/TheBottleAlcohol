import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/components/ui/sheet";
import { Button } from "@/components/ui/button";
import ProductFilterComponent from "@/components/ProductFilter";
import { Brand } from "@/lib/services/brandService";
import { Filter } from "lucide-react";
import { ProductFilter as FilterState } from "@/lib/services/productService";

interface FilterSortSheetProps {
  filters: FilterState;
  setFilters: React.Dispatch<React.SetStateAction<FilterState>>;
  brands: Brand[];
  totalProducts: number;
  isLoadingBrands: boolean;
}

export function FilterSortSheet({
  filters,
  setFilters,
  brands,
  totalProducts,
  isLoadingBrands,
}: FilterSortSheetProps) {
  return (
    <Sheet>
      <SheetTrigger asChild>
        <Button variant="outline" className="md:hidden flex items-center gap-2">
          <Filter size={16} />
          Filter & Sort
        </Button>
      </SheetTrigger>
      <SheetContent>
        <SheetHeader>
          <SheetTitle>Filter & Sort</SheetTitle>
          <SheetDescription>Showing {totalProducts} products</SheetDescription>
        </SheetHeader>
        <div className="py-4">
          <ProductFilterComponent
            filters={filters}
            setFilters={setFilters}
            brands={brands}
            isLoadingBrands={isLoadingBrands}
          />
        </div>
      </SheetContent>
    </Sheet>
  );
}
