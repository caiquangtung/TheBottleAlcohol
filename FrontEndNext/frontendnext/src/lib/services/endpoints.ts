export const API_ENDPOINTS = {
  // Products
  PRODUCTS: "/products",
  PRODUCT_DETAIL: (id: string) => `/products/${id}`,

  // Categories
  CATEGORIES: "/categories",
  CATEGORY_DETAIL: (id: string) => `/categories/${id}`,

  // Auth
  LOGIN: "/auth/login",
  REGISTER: "/auth/register",
  LOGOUT: "/auth/logout",
  PROFILE: "/auth/profile",

  // User
  USER_PROFILE: "/user/profile",
  USER_ORDERS: "/user/orders",
} as const;
