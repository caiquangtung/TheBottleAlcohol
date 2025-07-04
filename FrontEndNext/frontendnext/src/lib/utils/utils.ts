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

/**
 * Tạo slug từ chuỗi tiếng Việt
 * Ví dụ: "Rượu Đầu Đá" → "ruou-dau-da"
 */
export const generateSlug = (text: string): string => {
  if (!text) return "";

  // Map các ký tự tiếng Việt có dấu thành không dấu
  const vietnameseMap: { [key: string]: string } = {
    à: "a",
    á: "a",
    ả: "a",
    ã: "a",
    ạ: "a",
    ă: "a",
    ằ: "a",
    ắ: "a",
    ẳ: "a",
    ẵ: "a",
    ặ: "a",
    â: "a",
    ầ: "a",
    ấ: "a",
    ẩ: "a",
    ẫ: "a",
    ậ: "a",
    è: "e",
    é: "e",
    ẻ: "e",
    ẽ: "e",
    ẹ: "e",
    ê: "e",
    ề: "e",
    ế: "e",
    ể: "e",
    ễ: "e",
    ệ: "e",
    ì: "i",
    í: "i",
    ỉ: "i",
    ĩ: "i",
    ị: "i",
    ò: "o",
    ó: "o",
    ỏ: "o",
    õ: "o",
    ọ: "o",
    ô: "o",
    ồ: "o",
    ố: "o",
    ổ: "o",
    ỗ: "o",
    ộ: "o",
    ơ: "o",
    ờ: "o",
    ớ: "o",
    ở: "o",
    ỡ: "o",
    ợ: "o",
    ù: "u",
    ú: "u",
    ủ: "u",
    ũ: "u",
    ụ: "u",
    ư: "u",
    ừ: "u",
    ứ: "u",
    ử: "u",
    ữ: "u",
    ự: "u",
    ỳ: "y",
    ý: "y",
    ỷ: "y",
    ỹ: "y",
    ỵ: "y",
    đ: "d",
    À: "A",
    Á: "A",
    Ả: "A",
    Ã: "A",
    Ạ: "A",
    Ă: "A",
    Ằ: "A",
    Ắ: "A",
    Ẳ: "A",
    Ẵ: "A",
    Ặ: "A",
    Â: "A",
    Ầ: "A",
    Ấ: "A",
    Ẩ: "A",
    Ẫ: "A",
    Ậ: "A",
    È: "E",
    É: "E",
    Ẻ: "E",
    Ẽ: "E",
    Ẹ: "E",
    Ê: "E",
    Ề: "E",
    Ế: "E",
    Ể: "E",
    Ễ: "E",
    Ệ: "E",
    Ì: "I",
    Í: "I",
    Ỉ: "I",
    Ĩ: "I",
    Ị: "I",
    Ò: "O",
    Ó: "O",
    Ỏ: "O",
    Õ: "O",
    Ọ: "O",
    Ô: "O",
    Ồ: "O",
    Ố: "O",
    Ổ: "O",
    Ỗ: "O",
    Ộ: "O",
    Ơ: "O",
    Ờ: "O",
    Ớ: "O",
    Ở: "O",
    Ỡ: "O",
    Ợ: "O",
    Ù: "U",
    Ú: "U",
    Ủ: "U",
    Ũ: "U",
    Ụ: "U",
    Ư: "U",
    Ừ: "U",
    Ứ: "U",
    Ử: "U",
    Ữ: "U",
    Ự: "U",
    Ỳ: "Y",
    Ý: "Y",
    Ỷ: "Y",
    Ỹ: "Y",
    Ỵ: "Y",
    Đ: "D",
  };

  return text
    .split("")
    .map((char) => vietnameseMap[char] || char)
    .join("")
    .toLowerCase()
    .trim()
    .replace(/[^a-z0-9\s-]/g, "") // Loại bỏ ký tự đặc biệt (trừ chữ cái, số, khoảng trắng, dấu gạch)
    .replace(/\s+/g, "-") // Thay khoảng trắng bằng dấu gạch ngang
    .replace(/-+/g, "-") // Loại bỏ dấu gạch ngang liên tiếp
    .replace(/^-|-$/g, ""); // Loại bỏ dấu gạch ngang ở đầu và cuối
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
