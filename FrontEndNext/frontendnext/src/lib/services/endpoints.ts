export const API_ENDPOINTS = {
  // Products
  PRODUCTS: "/product",
  PRODUCT_DETAIL: (id: string) => `/product/${id}`,

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
} as const;
