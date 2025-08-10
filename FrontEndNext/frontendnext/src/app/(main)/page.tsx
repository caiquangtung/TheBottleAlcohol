"use client";
import Banner from "../../components/home/Banner";
import CategorySection from "../../components/home/CategorySection";
import ProductSection from "../../components/home/ProductSection";

export default function Home() {
  return (
    <div className="min-h-screen flex flex-col bg-background text-foreground transition-colors duration-300">
      <Banner />
      <CategorySection />
      <ProductSection />
    </div>
  );
}
