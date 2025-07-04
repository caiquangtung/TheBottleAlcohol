export const API_ENDPOINTS = {
  // Products
  PRODUCTS: "/product",
  PRODUCT_DETAIL: (id: string) => `/product/${id}`,
  PRODUCTS_BY_BRAND: (brandId: number) => `/product/brand/${brandId}`,
  PRODUCTS_BY_IDS: "/product/list-by-ids",

  // Categories
  CATEGORIES: "/category",
  CATEGORY_DETAIL: (id: string) => `/category/${id}`,

  // Brands
  BRANDS: "/brand",

  // Cart
  CART: "/cart",
  CART_CURRENT: "/cart/current",
  CART_SYNC: "/cart/sync",

  // Auth
  LOGIN: "/auth/login",
  REGISTER: "/auth/register",
  LOGOUT: "/auth/logout",
  PROFILE: "/auth/profile",

  // User
  USER_PROFILE: "/user/profile",
  USER_ORDERS: "/user/orders",

  // Wishlist
  WISHLISTS: "/wishlist",
  WISHLIST_DETAIL: (id: number | string) => `/wishlist/${id}`,
  WISHLIST_BY_CUSTOMER: (customerId: number | string) =>
    `/wishlist/customer/${customerId}`,
  WISHLIST_PRODUCTS: (wishlistId: number | string) =>
    `/wishlist/${wishlistId}/products`,
  WISHLIST_PRODUCT_DETAIL: (
    wishlistId: number | string,
    productId: number | string
  ) => `/wishlist/${wishlistId}/products/${productId}`,

  // Dashboard
  DASHBOARD_SUMMARY: "/dashboard/summary",

  // Users (Account)
  USERS: "/account",
  USER_DETAIL: (id: number | string) => `/account/${id}`,
  USER_CREATE: "/account/create-with-role",
  USER_UPDATE: (id: number | string) => `/account/${id}`,
  USER_DELETE: (id: number | string) => `/account/${id}`,

  // Products (Admin)
  PRODUCTS_ADMIN: "/product",
  PRODUCT_CREATE: "/product",
  PRODUCT_UPDATE: (id: number | string) => `/product/${id}`,
  PRODUCT_DELETE: (id: number | string) => `/product/${id}`,
} as const;
