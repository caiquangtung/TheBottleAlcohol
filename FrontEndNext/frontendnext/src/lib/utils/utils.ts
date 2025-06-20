import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

// Helper function to convert PascalCase to camelCase
export const toCamelCase = (str: string): string => {
  if (str.length === 0) return "";
  return str.charAt(0).toLowerCase() + str.slice(1);
};

// Helper function to transform object keys from PascalCase to camelCase
export const transformToCamelCase = (obj: any): any => {
  if (Array.isArray(obj)) {
    return obj.map(transformToCamelCase);
  }
  if (obj !== null && typeof obj === "object") {
    return Object.fromEntries(
      Object.entries(obj).map(([key, value]) => [
        toCamelCase(key),
        transformToCamelCase(value),
      ])
    );
  }
  return obj;
};

export const transformApiResponse = (response: any) => {
  const camelCasedResponse = transformToCamelCase(response);
  if (camelCasedResponse.success) {
    return camelCasedResponse.data;
  }
  // TODO: better error handling
  console.error("API Error:", camelCasedResponse.message || "Unknown error");
  return null;
};
