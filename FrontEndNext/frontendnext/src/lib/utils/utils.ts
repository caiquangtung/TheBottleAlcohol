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
export const transformToCamelCase = (obj: unknown): unknown => {
  if (Array.isArray(obj)) {
    return obj.map(transformToCamelCase);
  }
  if (obj !== null && typeof obj === "object") {
    return Object.fromEntries(
      Object.entries(obj as Record<string, unknown>).map(([key, value]) => [
        toCamelCase(key),
        transformToCamelCase(value),
      ])
    );
  }
  return obj;
};

export const transformApiResponse = <T = unknown>(response: unknown): T => {
  // Convert response to camelCase
  const camelCasedResponse = transformToCamelCase(response);

  // Case 1: Response with success flag and data
  if (
    camelCasedResponse &&
    typeof camelCasedResponse === "object" &&
    "success" in camelCasedResponse &&
    camelCasedResponse.success &&
    "data" in camelCasedResponse &&
    camelCasedResponse.data
  ) {
    return camelCasedResponse.data as T;
  }

  // Case 2: Success messages (like delete confirmations)
  if (
    camelCasedResponse &&
    typeof camelCasedResponse === "object" &&
    "success" in camelCasedResponse &&
    camelCasedResponse.success
  ) {
    return {
      success: true,
      message: (camelCasedResponse as { message?: string }).message,
    } as T;
  }

  // Case 3: Error messages
  if (
    camelCasedResponse &&
    typeof camelCasedResponse === "object" &&
    "success" in camelCasedResponse &&
    camelCasedResponse.success === false &&
    "message" in camelCasedResponse &&
    (camelCasedResponse as { message?: string }).message
  ) {
    console.error(
      "API Error:",
      (camelCasedResponse as { message?: string }).message
    );
    return {
      success: false,
      error: (camelCasedResponse as { message?: string }).message,
    } as T;
  }

  // Case 4: Fallback for other response formats
  return camelCasedResponse as T;
};
