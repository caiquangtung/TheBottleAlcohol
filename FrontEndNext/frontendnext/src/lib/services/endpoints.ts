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
} as const;
