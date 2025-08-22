import "../../styles/globals.css";
import type { Metadata } from "next";
import { Inter } from "next/font/google";
import { Providers } from "../../lib/store/Providers";
import ThemeSync from "../../lib/features/theme/ThemeSync";
import LocaleApplier from "@/lib/features/locale/LocaleApplier";
import { Toaster } from "sonner";
import AppFrame from "@/components/layout/AppFrame";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Alcohol Web",
  description: "Alcohol Web Application",
};

export default function MainLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <Providers>
          <ThemeSync />
          <LocaleApplier />
          <AppFrame>{children}</AppFrame>
        </Providers>
        <Toaster position="top-center" richColors />
      </body>
    </html>
  );
}
