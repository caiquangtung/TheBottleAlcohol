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

  // Suppliers
  SUPPLIERS: "/supplier",

  // Discounts
  DISCOUNTS: "/discount",

  // Inventory
  INVENTORY: "/inventory",

  // Import Orders
  IMPORT_ORDERS: "/importorder",
  

  // Recipes
  RECIPES: "/recipe",

  // Reviews
  REVIEWS: "/review",

  // Notifications
  NOTIFICATIONS: "/notification",

  // Cart
  CART: "/cart",
  CART_BY_CUSTOMER: (customerId: number) => `/cart/customer/${customerId}`,
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
  // New simplified endpoints
  MY_WISHLIST_PRODUCTS: "/wishlist/products",
  ADD_TO_WISHLIST: (productId: number | string) =>
    `/wishlist/products/${productId}`,
  REMOVE_FROM_WISHLIST: (productId: number | string) =>
    `/wishlist/products/${productId}`,

  // Dashboard
  DASHBOARD_SUMMARY: "/dashboard/summary",

  // Users (Account)
  USERS: "/account",
  USER_DETAIL: (id: number | string) => `/account/${id}`,
  USER_CREATE: "/account/create-with-role",
  USER_UPDATE: (id: number | string) => `/account/${id}`,
  USER_DELETE: (id: number | string) => `/account/${id}`,

  // Orders
  ORDERS: "/order",
  ORDER_DETAIL: (id: number | string) => `/order/${id}`,
  ORDER_UPDATE_STATUS: (id: number | string) => `/order/${id}/status`,

  // VNPAY
  VNPAY_CREATE_PAYMENT: "/vnpay/create-payment",
  VNPAY_PAYMENT_RETURN: (queryString: string) =>
    `/vnpay/payment-return?${queryString}`,
  VNPAY_PAYMENT_STATUS: (orderId: number | string) =>
    `/vnpay/payment-status/${orderId}`,

  // Products (Admin)
  PRODUCTS_ADMIN: "/product",
  PRODUCT_CREATE: "/product",
  PRODUCT_UPDATE: (id: number | string) => `/product/${id}`,
  PRODUCT_DELETE: (id: number | string) => `/product/${id}`,
} as const;
