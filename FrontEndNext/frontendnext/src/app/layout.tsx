import "../styles/globals.css";
import type { Metadata } from "next";
import { Inter } from "next/font/google";
import { Providers } from "../lib/store/Providers";
import ThemeSync from "../lib/features/theme/ThemeSync";
import { Toaster } from "sonner";
import Header from "../components/layout/Header";
import Footer from "../components/layout/Footer";
import ClientLayoutShell from "../components/layout/ClientLayoutShell";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Alcohol Web",
  description: "Alcohol Web Application",
};

export default function RootLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <html lang="en">
      <body className={inter.className}>
        <Providers>
          <ThemeSync />
          <div className="flex flex-col min-h-screen">
            <Header />
            <ClientLayoutShell>{children}</ClientLayoutShell>
            <Footer />
          </div>
        </Providers>
        <Toaster position="top-center" richColors />
      </body>
    </html>
  );
}
