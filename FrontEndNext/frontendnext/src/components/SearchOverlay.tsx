"use client";
import { useAppDispatch, useAppSelector } from "@/lib/store/hooks";
import {
  closeSearchOverlay,
  setSearchTerm,
  clearSearch,
} from "@/lib/features/search/searchSlice";
import { useGetProductsQuery } from "@/lib/services/productService";
import { useGetAllBrandsQuery } from "@/lib/services/brandService";
import { useGetAllCategoriesQuery } from "@/lib/services/categoryService";
import { useGetRecipesQuery } from "@/lib/services/recipeService";
import { Button } from "./ui/button";
import { X } from "lucide-react";
import { useEffect, useState, useMemo } from "react";
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
  const { isSearchOverlayOpen, searchTerm } = useAppSelector(
    (state) => state.search
  );
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useState("");

  // Debounce search term
  useEffect(() => {
    const timer = setTimeout(() => {
      setDebouncedSearchTerm(searchTerm);
    }, 500);

    return () => clearTimeout(timer);
  }, [searchTerm]);

  // Lock/unlock body scroll when overlay is open/closed
  useEffect(() => {
    if (isSearchOverlayOpen) {
      // Save current scroll position
      const scrollY = window.scrollY;

      // Apply styles to prevent scrolling
      document.body.style.position = "fixed";
      document.body.style.top = `-${scrollY}px`;
      document.body.style.width = "100%";
      document.body.style.overflow = "hidden";

      return () => {
        // Restore scroll position and remove styles
        document.body.style.position = "";
        document.body.style.top = "";
        document.body.style.width = "";
        document.body.style.overflow = "";

        // Restore scroll position
        window.scrollTo(0, scrollY);
      };
    }
  }, [isSearchOverlayOpen]);

  // API Queries
  const {
    data: searchResults,
    isLoading: isSearching,
    isFetching,
    error: searchError,
  } = useGetProductsQuery(
    {
      SearchTerm: debouncedSearchTerm,
      pageNumber: 1,
      pageSize: 8,
    },
    {
      skip: !debouncedSearchTerm || debouncedSearchTerm.length < 2,
    }
  );

  // Get default products when no search
  const {
    data: defaultProducts,
    isLoading: isLoadingDefault,
    error: defaultError,
  } = useGetProductsQuery(
    {
      pageNumber: 1,
      pageSize: 8,
    },
    {
      skip: Boolean(debouncedSearchTerm && debouncedSearchTerm.length >= 2),
    }
  );

  const { data: allBrands = [] } = useGetAllBrandsQuery();
  const { data: allCategories = [] } = useGetAllCategoriesQuery();
  const { data: allRecipes = [] } = useGetRecipesQuery();

  // Filter data based on search term
  const filteredData = useMemo(() => {
    // Ensure data is arrays
    const brandsArray = Array.isArray(allBrands) ? allBrands : [];
    const categoriesArray = Array.isArray(allCategories) ? allCategories : [];
    const recipesArray = Array.isArray(allRecipes) ? allRecipes : [];

    if (!searchTerm || searchTerm.length < 2) {
      return {
        relatedSearches: popularSearches.slice(0, 9),
        relatedCollections: [
          ...brandsArray.slice(0, 4),
          ...categoriesArray.slice(0, 4),
        ],
        relatedArticles: recipesArray.slice(0, 4),
      };
    }

    const searchLower = searchTerm.toLowerCase();

    // Filter related searches from popularSearches that contain the search term
    const relatedSearches = popularSearches
      .filter((search) => search.toLowerCase().includes(searchLower))
      .slice(0, 9);

    // Add search term variations if no matches
    if (relatedSearches.length === 0) {
      relatedSearches.push(
        searchTerm.toUpperCase(),
        `${searchTerm.toUpperCase()} COCKTAIL`,
        `${searchTerm.toUpperCase()} RECIPE`
      );
    }

    // Filter brands and categories that contain search term
    const relatedBrands = brandsArray
      .filter(
        (brand) => brand.name && brand.name.toLowerCase().includes(searchLower)
      )
      .slice(0, 4);

    const relatedCategories = categoriesArray
      .filter(
        (category) =>
          category.name && category.name.toLowerCase().includes(searchLower)
      )
      .slice(0, 4);

    const relatedCollections = [...relatedBrands, ...relatedCategories];

    // Filter recipes that contain search term
    const relatedArticles = recipesArray
      .filter(
        (recipe) =>
          (recipe.name && recipe.name.toLowerCase().includes(searchLower)) ||
          (recipe.description &&
            recipe.description.toLowerCase().includes(searchLower))
      )
      .slice(0, 4);

    return {
      relatedSearches,
      relatedCollections,
      relatedArticles,
    };
  }, [searchTerm, allBrands, allCategories, allRecipes]);

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

  const handlePopularSearchClick = (term: string) => {
    dispatch(setSearchTerm(term));
    // Scroll to top to show search results
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  if (!isSearchOverlayOpen) {
    return null;
  }

  const isShowingSearchResults = Boolean(
    debouncedSearchTerm && debouncedSearchTerm.length >= 2
  );

  return (
    <div className="fixed top-20 left-0 right-0 bottom-0 z-[90] bg-black/50 backdrop-blur-sm">
      {/* Overlay background */}
      <div className="absolute inset-0" onClick={handleClose} />

      {/* Search content */}
      <div className="relative bg-white dark:bg-gray-900 min-h-full pt-8 overflow-y-auto">
        {/* Close button */}
        <Button
          variant="ghost"
          size="icon"
          className="absolute top-4 right-4 z-10"
          onClick={handleClose}
        >
          <X className="h-6 w-6" />
        </Button>

        {/* Content */}
        <div className="container mx-auto px-4 py-8">
          <div className="max-w-6xl mx-auto">
            {/* 2 Columns Layout: Main + Sidebar */}
            <div className="flex flex-col lg:flex-row gap-8">
              {/* Main Content - Left Side */}
              <div className="flex-1">
                {isShowingSearchResults ? (
                  /* Search Results */
                  <div>
                    <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide mb-6">
                      Search Results for "{debouncedSearchTerm}"
                      {searchResults && ` (${searchResults.totalRecords})`}
                    </h3>
                    {isSearching || isFetching ? (
                      <div className="flex justify-center py-8">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
                      </div>
                    ) : searchResults && searchResults.items.length > 0 ? (
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
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
                                  src={
                                    product.imageUrl ||
                                    "/placeholder-product.jpg"
                                  }
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
                      <div className="text-center py-12">
                        <h2 className="text-2xl font-bold text-gray-900 dark:text-white mb-4">
                          NO RESULTS FOUND
                        </h2>
                        <p className="text-gray-500 dark:text-gray-400 mb-6">
                          Unfortunately we couldn't find any results for '
                          {debouncedSearchTerm}'
                        </p>
                        <button
                          onClick={() => dispatch(clearSearch())}
                          className="bg-gray-900 dark:bg-white text-white dark:text-gray-900 px-6 py-3 font-semibold uppercase tracking-wide hover:bg-gray-800 dark:hover:bg-gray-100 transition-colors"
                        >
                          SHOP TRENDING PRODUCTS
                        </button>
                      </div>
                    )}
                  </div>
                ) : (
                  /* Default Products */
                  <div>
                    <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide mb-6">
                      Featured Products
                    </h3>
                    {isLoadingDefault ? (
                      <div className="flex justify-center py-8">
                        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
                      </div>
                    ) : defaultProducts && defaultProducts.items.length > 0 ? (
                      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
                        {defaultProducts.items.map((product) => (
                          <Link
                            key={product.id}
                            href={`/product/${product.slug}-${product.id}`}
                            onClick={() => dispatch(closeSearchOverlay())}
                            className="group"
                          >
                            <div className="bg-white dark:bg-gray-800 rounded-lg shadow-sm hover:shadow-md transition-shadow duration-200 overflow-hidden">
                              <div className="aspect-square relative">
                                <Image
                                  src={
                                    product.imageUrl ||
                                    "/placeholder-product.jpg"
                                  }
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
                      <div className="text-center py-8">
                        <p className="text-gray-500 dark:text-gray-400">
                          No products available
                        </p>
                      </div>
                    )}
                  </div>
                )}
              </div>

              {/* Sidebar - Right Side */}
              <div className="w-full lg:w-80 space-y-8">
                {/* Popular Searches */}
                <div className="space-y-4">
                  <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide">
                    {Boolean(searchTerm && searchTerm.length >= 2)
                      ? "RELATED SEARCHES"
                      : "POPULAR SEARCHES"}
                  </h3>
                  <div className="flex flex-wrap gap-2">
                    {filteredData.relatedSearches.map((term, index) => (
                      <button
                        key={`${term}-${index}`}
                        onClick={() => handlePopularSearchClick(term)}
                        className="px-3 py-1 text-xs font-medium text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-800 hover:bg-gray-200 dark:hover:bg-gray-700 rounded-full transition-colors"
                      >
                        {term}
                      </button>
                    ))}
                  </div>
                </div>

                {/* Popular Collections */}
                <div className="space-y-4">
                  <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide">
                    {Boolean(searchTerm && searchTerm.length >= 2)
                      ? "RELATED COLLECTIONS"
                      : "POPULAR COLLECTIONS"}
                  </h3>
                  <div className="flex flex-wrap gap-2">
                    {filteredData.relatedCollections.length > 0
                      ? filteredData.relatedCollections.map((item, index) => {
                          if (!item.name) return null;
                          return (
                            <button
                              key={`${item.name || item.id}-${index}`}
                              onClick={() =>
                                handlePopularSearchClick(item.name)
                              }
                              className="px-3 py-1 text-xs font-medium text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-800 hover:bg-gray-200 dark:hover:bg-gray-700 rounded-full transition-colors"
                            >
                              {item.name.toUpperCase()}
                            </button>
                          );
                        })
                      : /* Fallback to static data if no filtered results */
                        popularCollections.slice(0, 8).map((collection) => (
                          <button
                            key={collection}
                            onClick={() => handlePopularSearchClick(collection)}
                            className="px-3 py-1 text-xs font-medium text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-800 hover:bg-gray-200 dark:hover:bg-gray-700 rounded-full transition-colors"
                          >
                            {collection}
                          </button>
                        ))}
                  </div>
                </div>

                {/* Popular Articles */}
                <div className="space-y-4">
                  <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide">
                    {Boolean(searchTerm && searchTerm.length >= 2)
                      ? "RELATED ARTICLES"
                      : "POPULAR ARTICLES"}
                  </h3>
                  <div className="flex flex-wrap gap-2">
                    {filteredData.relatedArticles.length > 0
                      ? filteredData.relatedArticles.map((recipe, index) => {
                          if (!recipe.name || !recipe.id) return null;
                          const slug = recipe.name
                            .toLowerCase()
                            .replace(/\s+/g, "-")
                            .replace(/[^\w-]/g, "");
                          return (
                            <Link
                              key={`${recipe.id}-${index}`}
                              href={`/recipe/${slug}-${recipe.id}`}
                              onClick={() => dispatch(closeSearchOverlay())}
                              className="px-3 py-1 text-xs font-medium text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-800 hover:bg-gray-200 dark:hover:bg-gray-700 rounded-full transition-colors"
                            >
                              {recipe.name.toUpperCase()}
                            </Link>
                          );
                        })
                      : /* Fallback to static data if no filtered results */
                        popularArticles.map((article) => (
                          <button
                            key={article}
                            onClick={() => handlePopularSearchClick(article)}
                            className="px-3 py-1 text-xs font-medium text-gray-700 dark:text-gray-300 bg-gray-100 dark:bg-gray-800 hover:bg-gray-200 dark:hover:bg-gray-700 rounded-full transition-colors"
                          >
                            {article}
                          </button>
                        ))}
                  </div>
                </div>

                {/* Help Section */}
                <div className="space-y-4">
                  <h3 className="text-lg font-semibold text-gray-900 dark:text-white uppercase tracking-wide">
                    HOW CAN WE HELP?
                  </h3>
                  <div className="space-y-2">
                    <button className="block w-full text-left text-sm text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
                      Delivery & Returns
                    </button>
                    <button className="block w-full text-left text-sm text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
                      Terms & Conditions
                    </button>
                    <button className="block w-full text-left text-sm text-gray-600 dark:text-gray-300 hover:text-blue-600 dark:hover:text-blue-400 transition-colors">
                      Recipe Library
                    </button>
                    <button className="block w-full text-left text-sm text-gray-600 dark:hover:text-blue-400 transition-colors">
                      The Drinks Journal
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
