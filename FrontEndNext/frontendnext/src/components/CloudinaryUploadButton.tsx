"use client";

import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import Image from "next/image";
import { cloudinaryConfig, isCloudinaryConfigured } from "@/lib/cloudinary";
import { Button } from "@/components/ui/button";

type CloudinaryResult = {
  secure_url: string;
  url: string;
  public_id: string;
  resource_type: string;
  format: string;
  bytes: number;
  width: number;
  height: number;
};

export function CloudinaryUploadButton({
  value,
  onChange,
  label = "Upload Image",
  disabled,
}: {
  value?: string;
  onChange: (url: string, meta?: CloudinaryResult) => void;
  label?: string;
  disabled?: boolean;
}) {
  const [isLoading, setIsLoading] = useState(false);
  const inputRef = useRef<HTMLInputElement | null>(null);

  const canUse = isCloudinaryConfigured();

  const upload = useCallback(
    async (file: File) => {
      if (!canUse) return;
      setIsLoading(true);
      try {
        const form = new FormData();
        form.append("file", file);
        form.append("upload_preset", cloudinaryConfig.uploadPreset);
        if (cloudinaryConfig.folder) {
          form.append("folder", cloudinaryConfig.folder);
        }

        const endpoint = `https://api.cloudinary.com/v1_1/${cloudinaryConfig.cloudName}/image/upload`;
        const res = await fetch(endpoint, { method: "POST", body: form });
        if (!res.ok) throw new Error(`Upload failed: ${res.status}`);
        const data = (await res.json()) as CloudinaryResult;
        onChange(data.secure_url || data.url, data);
      } catch (e) {
        console.error(e);
      } finally {
        setIsLoading(false);
      }
    },
    [onChange, canUse]
  );

  const onPick = useCallback(() => {
    inputRef.current?.click();
  }, []);

  const onFileChange = useCallback(
    (e: React.ChangeEvent<HTMLInputElement>) => {
      const file = e.target.files?.[0];
      if (file) upload(file);
      e.currentTarget.value = ""; // reset to allow re-selecting same file
    },
    [upload]
  );

  return (
    <div className="space-y-2">
      <div className="flex items-center gap-3">
        <Button
          type="button"
          onClick={onPick}
          disabled={disabled || !canUse || isLoading}
        >
          {isLoading ? "Uploading..." : label}
        </Button>
        {!canUse && (
          <span className="text-xs text-red-500">
            Cloudinary env not configured
          </span>
        )}
      </div>
      {value && (
        <div className="relative w-40 h-40 rounded overflow-hidden border">
          <Image
            src={value}
            alt="preview"
            fill
            sizes="160px"
            className="object-cover"
          />
        </div>
      )}
      <input
        ref={inputRef}
        type="file"
        accept="image/*"
        className="hidden"
        onChange={onFileChange}
      />
    </div>
  );
}

export default CloudinaryUploadButton;
