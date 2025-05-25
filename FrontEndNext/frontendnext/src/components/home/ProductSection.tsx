"use client";
import Image from "next/image";
import { Card } from "../ui/card";
import { Badge } from "../ui/badge";
import { Button } from "../ui/button";
import { Heart } from "lucide-react";

const products = [
  {
    brand: "BAILEYS",
    name: "TOFFEE POPCORN",
    type: "IRISH CREAM",
    price: "£19.99",
    volume: "70CL",
    abv: "17.0%",
    country: "IRE | DUBLIN",
    img: "/product.png",
    limited: true,
    rating: null,
  },
  {
    brand: "AU VODKA",
    name: "JUICY PEACH",
    type: "PEACH VODKA",
    price: "£31.99",
    volume: "70CL",
    abv: "35.0%",
    country: "ENG | LONDON",
    img: "/product.png",
    limited: true,
    rating: null,
  },
  {
    brand: "HENNESSY",
    name: "300TH FOUNDER'S EDITION",
    type: "VS COGNAC",
    price: "£37.49",
    volume: "70CL",
    abv: "40.0%",
    country: "FRA | COGNAC",
    img: "/product.png",
    limited: true,
    rating: null,
  },
  {
    brand: "SMIRNOFF",
    name: "MIAMI PEACH",
    type: "PEACH VODKA",
    price: "£18.49",
    volume: "70CL",
    abv: "37.5%",
    country: "ENG | LONDON",
    img: "/product.png",
    limited: true,
    rating: 5,
  },
  {
    brand: "DON JULIO",
    name: "PEGGY GOU",
    type: "ANEJO TEQUILA",
    price: "£197.49",
    volume: "70CL",
    abv: "38%",
    country: "MEX",
    img: "/product.png",
    limited: true,
    rating: null,
  },
  {
    brand: "CIROC",
    name: "STRAWBERRY LEMONADE",
    type: "STRAWBERRY VODKA",
    price: "£31.49",
    volume: "70CL",
    abv: "30%",
    country: "FRA",
    img: "/product.png",
    limited: true,
    rating: null,
  },
];

export default function ProductSection() {
  return (
    <section className="container mx-auto py-8">
      <h2 className="text-2xl md:text-3xl font-bold text-center mb-2">
        OUR TOP PICKS OF THE WEEK
      </h2>
      <p className="text-center text-muted-foreground mb-8">
        We&apos;ve handpicked the newest bottles making noise for all the right
        reasons. From small-batch spirits to buzzy collabs—consider this your
        drinks radar, updated.
      </p>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
        {/* Left: Big promo card */}
        <Card
          className="md:col-span-1 flex flex-col justify-between bg-cover bg-center min-h-[520px] relative dark:bg-[#18181b]"
          style={{ backgroundImage: "url(/promo-hennessy.png)" }}
        >
          <a
            href="/collections/new-arrivals"
            title="Fresh Drops Worth The Hype"
            className="absolute inset-0 w-full h-full z-10"
            tabIndex={-1}
            aria-label="Fresh Drops Worth The Hype"
          ></a>
          <div className="relative z-20 flex flex-col h-full justify-end p-8">
            <h3 className="text-4xl font-bold text-white mb-6 drop-shadow-lg">
              FRESH DROPS WORTH THE HYPE
            </h3>
            <Button
              variant="secondary"
              className="w-fit dark:bg-[#f96d8d] dark:text-black"
            >
              Shop Now
            </Button>
          </div>
        </Card>
        {/* Right: Product grid */}
        <div className="md:col-span-2 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {products.map((prod, i) => (
            <Card
              key={i}
              className="relative flex flex-col items-start p-4 gap-0 dark:bg-[#23232b]"
            >
              {prod.limited && (
                <Badge className="absolute top-2 left-2 dark:bg-[#3a2a3a] dark:text-[#f96d8d] !px-3 !py-1 text-xs font-semibold tracking-wide">
                  LIMITED EDITION
                </Badge>
              )}
              {/* Heart icon top right */}
              <Button
                variant="ghost"
                size="icon"
                className="absolute top-2 right-2"
              >
                <Heart className="h-5 w-5" />
              </Button>
              <Image
                src={prod.img}
                alt={prod.name}
                width={300}
                height={400}
                className="block mx-auto mb-2 w-full h-auto max-h-60 object-contain"
              />
              <span className="text-xs text-muted-foreground uppercase tracking-wide mb-0.5">
                {prod.brand}
              </span>
              <span className="font-bold text-lg mb-0.5">{prod.name}</span>
              <span className="text-xs text-muted-foreground uppercase tracking-wide mb-1">
                {prod.type}
              </span>
              <div className="flex items-center justify-between w-full mb-1">
                <span className="font-bold text-xl">{prod.price}</span>
                <div className="flex items-center gap-1">
                  {[...Array(5)].map((_, idx) => (
                    <span
                      key={idx}
                      className={
                        idx < (prod.rating || 0)
                          ? "text-yellow-400"
                          : "text-gray-300"
                      }
                    >
                      ★
                    </span>
                  ))}
                </div>
              </div>
              <div className="flex flex-wrap gap-2 text-xs text-muted-foreground mb-2">
                <span>{prod.volume}</span>
                <span>|</span>
                <span>{prod.abv}</span>
                <span>|</span>
                <span>{prod.country}</span>
              </div>
              <Button
                size="lg"
                className="w-full border-2 border-black bg-transparent text-black dark:bg-[#f96d8d] dark:text-black dark:border-white font-bold tracking-wide mt-2 hover:bg-black hover:text-white dark:hover:bg-white dark:hover:text-black"
              >
                ADD TO CART
              </Button>
            </Card>
          ))}
        </div>
      </div>
    </section>
  );
}
