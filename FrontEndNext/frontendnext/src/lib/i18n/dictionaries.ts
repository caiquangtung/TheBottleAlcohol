import { TranslationDict } from "./types";

export const vi: TranslationDict = {
  common: {
    searchPlaceholder: "Tìm kiếm...",
    noResultsTitle: "Không tìm thấy kết quả",
    noResultsDesc: "Rất tiếc, không có kết quả cho '{term}'",
    trendingButton: "XEM SẢN PHẨM NỔI BẬT",
    featuredProducts: "Sản phẩm nổi bật",
    relatedSearches: "TÌM KIẾM LIÊN QUAN",
    popularSearches: "TÌM KIẾM PHỔ BIẾN",
    relatedCollections: "BỘ SƯU TẬP LIÊN QUAN",
    popularCollections: "BỘ SƯU TẬP PHỔ BIẾN",
    relatedArticles: "BÀI VIẾT LIÊN QUAN",
    popularArticles: "BÀI VIẾT PHỔ BIẾN",
    howCanWeHelp: "CHÚNG TÔI CÓ THỂ GIÚP GÌ?",
    deliveryReturns: "Giao hàng & Đổi trả",
    terms: "Điều khoản & Điều kiện",
    recipeLibrary: "Thư viện công thức",
    journal: "Tạp chí đồ uống",
    searchResultsFor: 'Kết quả cho "{term}"',
  },
};

export const en: TranslationDict = {
  common: {
    searchPlaceholder: "Search...",
    noResultsTitle: "No results found",
    noResultsDesc: "Unfortunately we couldn't find any results for '{term}'",
    trendingButton: "SHOP TRENDING PRODUCTS",
    featuredProducts: "Featured Products",
    relatedSearches: "RELATED SEARCHES",
    popularSearches: "POPULAR SEARCHES",
    relatedCollections: "RELATED COLLECTIONS",
    popularCollections: "POPULAR COLLECTIONS",
    relatedArticles: "RELATED ARTICLES",
    popularArticles: "POPULAR ARTICLES",
    howCanWeHelp: "HOW CAN WE HELP?",
    deliveryReturns: "Delivery & Returns",
    terms: "Terms & Conditions",
    recipeLibrary: "Recipe Library",
    journal: "The Drinks Journal",
    searchResultsFor: 'Search Results for "{term}"',
  },
};

export const DICTS: Record<"vi" | "en", TranslationDict> = { vi, en };
