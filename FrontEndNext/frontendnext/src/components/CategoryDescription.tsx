"use client";
import { useState } from "react";

export interface CategoryDescriptionProps {
  title: string;
  metaDescription?: string | null;
  description: string;
  imageUrl?: string | null;
  breadcrumb?: { name: string; href: string }[];
}

export default function CategoryDescription({
  title,
  metaDescription,
  description,
  imageUrl,
  breadcrumb = [],
}: CategoryDescriptionProps) {
  const [expanded, setExpanded] = useState(false);
  const meta = metaDescription || description;
  const shortMeta = meta.slice(0, 180);

  return (
    <div className="relative w-full min-h-[320px] flex items-center justify-center mb-8">
      {imageUrl && (
        <div
          className="absolute inset-0 bg-cover bg-center z-0"
          style={{ backgroundImage: `url(${imageUrl})` }}
        >
          <div className="absolute inset-0 bg-black/60" />
        </div>
      )}
      <div className="relative z-10 w-full max-w-4xl px-6 py-12 text-center text-white">
        {breadcrumb.length > 0 && (
          <nav className="mb-2 text-sm text-white/80 flex flex-wrap justify-center gap-1">
            {breadcrumb.map((b, i) => (
              <span key={b.href} className="flex items-center gap-1">
                <a href={b.href} className="hover:underline">
                  {b.name}
                </a>
                {i < breadcrumb.length - 1 && <span className="mx-1">-</span>}
              </span>
            ))}
          </nav>
        )}
        <h1 className="text-4xl font-bold mb-4 drop-shadow-lg">{title}</h1>
        <div className="text-lg max-w-2xl mx-auto">
          {expanded || meta.length <= 180 ? meta : shortMeta + "..."}
          {meta.length > 180 && (
            <button
              className="ml-2 text-blue-200 underline text-sm"
              onClick={() => setExpanded((v) => !v)}
            >
              {expanded ? "Close" : "Read more"}
            </button>
          )}
        </div>
      </div>
    </div>
  );
}
