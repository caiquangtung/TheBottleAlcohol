"use client";
import { useAppDispatch, useAppSelector } from "@/lib/store/hooks";
import {
  closeSearchOverlay,
  setSearchTerm,
} from "@/lib/features/search/searchSlice";
import { useGetProductsQuery } from "@/lib/services/productService";
import { Input } from "./ui/input";
import { Button } from "./ui/button";
import { Badge } from "./ui/badge";
import { Search, X } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import Image from "next/image";
import Link from "next/link";

// Popular search data
const popularSearches = [
  "JOHNNIE WALKER",
  "MOËT & CHANDON",
  "LAURENT-PERRIER",
  "BAILEYS",
  "CIROC",
  "HENNESSY",
  "JACK DANIELS",
  "AU VODKA",
  "BELVEDERE",
];

const popularCollections = [
  "ALCOHOL MINIATURES",
  "ALCOHOL GIFTS",
  "OFFERS",
  "WHISKY",
  "VODKA",
  "GIN",
  "RUM",
  "WINE & CHAMPAGNE",
];

const popularArticles = [
  "TRASHCAN COCKTAIL RECIPE",
  "MONKEY BRAIN SHOT RECIPE",
  "NEGRONI COCKTAIL RECIPE",
  "SANGRIA COCKTAIL RECIPE",
];

export default function SearchOverlay() {
  const dispatch = useAppDispatch();
  const router = useRouter();
  const { isSearchOverlayOpen, searchTerm } = useAppSelector(
    (state) => state.search
  );
  const searchInputRef = useRef<HTMLInputElement>(null);
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState("");

  // Debounce search term
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm);
    }, 500);

    return () => clearTimeout(timer);
  }, [searchTerm]);

  // Search products query
  const {
    data: searchResults,
    isLoading: isSearching,
    isFetching,
  } = useGetProductsQuery(
    {
      search: debouncedSearchTerm,
      pageNumber: 1,
      pageSize: 8,
    },
    {
      skip: !debouncedSearchTerm || debouncedSearchTerm.length < 2,
    }
  );

  // Focus search input when overlay opens
  useEffect(() => {
    if (isSearchOverlayOpen && searchInputRef.current) {
      searchInputRef.current.focus();
    }
  }, [isSearchOverlayOpen]);

  // Handle escape key
  useEffect(() => {
    const handleEscape = (e: KeyboardEvent) => {
      if (e.key === "Escape" && isSearchOverlayOpen) {
        dispatch(closeSearchOverlay());
      }
    };

    document.addEventListener("keydown", handleEscape);
    return () => document.removeEventListener("keydown", handleEscape);
  }, [isSearchOverlayOpen, dispatch]);

  const handleClose = () => {
    dispatch(closeSearchOverlay());
  };

  const handleSearchTermChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    dispatch(setSearchTerm(e.target.value));
  };

  const handlePopularSearchClick = (term: string) => {
    dispatch(setSearchTerm(term));
    // Navigate to search results or trigger search
    console.log("Search for:", term);
  };

  if (!isSearchOverlayOpen) {
    return null;
  }

  return (
    <div className="fixed inset-0 z-50 bg-black/50 backdrop-blur-sm">
      {/* Overlay background */}
      <div className="absolute inset-0" onClick={handleClose} />

      {/* Search content */}
      <div className="relative bg-white dark:bg-gray-900 min-h-screen pt-20">
        {/* Close button */}
        <Button
          variant="ghost"
          size="icon"
          className="absolute top-4 right-4 z-10"
          onClick={handleClose}
        >
          <X className="h-6 w-6" />
        </Button>

        {/* Search input */}
        <div className="container mx-auto px-4 py-8">
          <div className="max-w-2xl mx-auto mb-12">
            <div className="relative">
              <Search className="absolute left-4 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
              <Input
                ref={searchInputRef}
                value={searchTerm}
                onChange={handleSearchTermChange}
                placeholder="Search for products, brands, recipes..."
                className="pl-12 pr-4 py-6 text-lg border-2 border-gray-200 dark:border-gray-700 rounded-lg focus:border-blue-500 dark:focus:border-blue-400"
              />
            </div>
          </div>

          {/* Search Results */}
          {debouncedSearchTerm && debouncedSearchTerm.length >= 2 && (
            <div className="max-w-6xl mx-auto mb-12">
              <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide mb-6">
                Search Results{" "}
                {searchResults && `(${searchResults.totalRecords})`}
              </h3>
              {isSearching || isFetching ? (
                <div className="flex justify-center py-8">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
                </div>
              ) : searchResults && searchResults.items.length > 0 ? (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  {searchResults.items.map((product) => (
                    <Link
                      key={product.id}
                      href={`/product/${product.slug}-${product.id}`}
                      onClick={() => dispatch(closeSearchOverlay())}
                      className="group"
                    >
                      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-sm hover:shadow-md transition-shadow duration-200 overflow-hidden">
                        <div className="aspect-square relative">
                          <Image
                            src={product.imageUrl || "/placeholder-product.jpg"}
                            alt={product.name}
                            fill
                            className="object-cover group-hover:scale-105 transition-transform duration-200"
                          />
                        </div>
                        <div className="p-4">
                          <h4
                            className="font-medium text-sm text-gray-900 dark:text-white mb-2 overflow-hidden"
                            style={{
                              display: "-webkit-box",
                              WebkitLineClamp: 2,
                              WebkitBoxOrient: "vertical" as const,
                            }}
                          >
                            {product.name}
                          </h4>
                          <p className="text-sm text-gray-500 dark:text-gray-400 mb-2">
                            {product.brandName}
                          </p>
                          <p className="text-lg font-bold text-gray-900 dark:text-white">
                            ${product.price.toFixed(2)}
                          </p>
                        </div>
                      </div>
                    </Link>
                  ))}
                </div>
              ) : (
                debouncedSearchTerm && (
                  <div className="text-center py-8">
                    <p className="text-gray-500 dark:text-gray-400">
                      No products found for "{debouncedSearchTerm}"
                    </p>
                  </div>
                )
              )}
            </div>
          )}

          {/* Content sections */}
          <div className="max-w-6xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
            {/* Popular Searches */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide">
                POPULAR SEARCHES
              </h3>
              <div className="space-y-2">
                {popularSearches.map((term) => (
                  <button
                    key={term}
                    onClick={() => handlePopularSearchClick(term)}
                    className="block w-full text-left text-sm text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                  >
                    {term}
                  </button>
                ))}
              </div>
            </div>

            {/* Popular Collections */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide">
                POPULAR COLLECTIONS
              </h3>
              <div className="space-y-2">
                {popularCollections.map((collection) => (
                  <button
                    key={collection}
                    onClick={() => handlePopularSearchClick(collection)}
                    className="block w-full text-left text-sm text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                  >
                    {collection}
                  </button>
                ))}
              </div>
            </div>

            {/* Popular Articles */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide">
                POPULAR ARTICLES
              </h3>
              <div className="space-y-2">
                {popularArticles.map((article) => (
                  <button
                    key={article}
                    onClick={() => handlePopularSearchClick(article)}
                    className="block w-full text-left text-sm text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors"
                  >
                    {article}
                  </button>
                ))}
              </div>
            </div>
          </div>

          {/* Help section */}
          <div className="max-w-6xl mx-auto mt-16 pt-8 border-t border-gray-200 dark:border-gray-700">
            <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide mb-4">
              HOW CAN WE HELP?
            </h3>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
              <button className="text-left text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
                Delivery & Returns
              </button>
              <button className="text-left text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
                Terms & Conditions
              </button>
              <button className="text-left text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
                Recipe Library
              </button>
              <button className="text-left text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
                The Drinks Journal
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
