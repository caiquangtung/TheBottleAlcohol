"use client";
import { useSelector } from "react-redux";
import { RootState } from "@/lib/store";
import { DICTS } from "./dictionaries";
import { TranslationParams } from "./types";

function format(template: string, params?: TranslationParams) {
  if (!params) return template;
  return template.replace(/\{(.*?)\}/g, (_, key) => {
    const v = params[key];
    return v === undefined || v === null ? "" : String(v);
  });
}

export function useI18n() {
  const locale = useSelector((s: RootState) => s.locale.current);
  const dict = DICTS[locale] ?? DICTS.vi;

  function t(path: string, params?: TranslationParams): string {
    const segments = path.split(".");
    let node: any = dict;
    for (const seg of segments) {
      node = node?.[seg];
      if (node === undefined) break;
    }
    if (typeof node === "string") return format(node, params);
    return path;
  }

  return { t, locale };
}
