export type CloudinaryConfig = {
  cloudName: string;
  uploadPreset: string;
  folder?: string;
};

export const cloudinaryConfig: CloudinaryConfig = {
  cloudName: process.env.NEXT_PUBLIC_CLOUDINARY_CLOUD_NAME || "",
  uploadPreset: process.env.NEXT_PUBLIC_CLOUDINARY_UPLOAD_PRESET || "",
  folder: process.env.NEXT_PUBLIC_CLOUDINARY_FOLDER || "",
};

export function isCloudinaryConfigured(): boolean {
  return Boolean(cloudinaryConfig.cloudName && cloudinaryConfig.uploadPreset);
}
