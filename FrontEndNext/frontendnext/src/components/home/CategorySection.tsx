"use client";
import Image from "next/image";
import { Avatar } from "../ui/avatar";

const categories = [
  { label: "Whisky", img: "/category.png" },
  { label: "Wine", img: "/category.png" },
  { label: "Vodka", img: "/category.png" },
  { label: "Gin", img: "/category.png" },
  { label: "New Arrivals", img: "/category.png" },
  { label: "Gifts", img: "/category.png" },
  { label: "Limited Edition", img: "/category.png" },
  { label: "Rum", img: "/category.png" },
];

export default function CategorySection() {
  return (
    <section className="container mx-auto py-8">
      <div className="flex flex-wrap justify-center gap-6">
        {categories.map((cat) => (
          <div key={cat.label} className="flex flex-col items-center gap-2">
            <Avatar className="bg-muted dark:bg-[#23232b]">
              <Image src={cat.img} alt={cat.label} width={64} height={64} />
            </Avatar>
            <span className="text-sm font-medium">{cat.label}</span>
          </div>
        ))}
      </div>
    </section>
  );
}
