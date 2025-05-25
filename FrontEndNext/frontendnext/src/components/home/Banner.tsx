"use client";
import Image from "next/image";
import { useEffect, useState } from "react";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselPrevious,
  CarouselNext,
} from "../ui/carousel";

export default function Banner() {
  const [current, setCurrent] = useState(0);
  const [carouselApi, setCarouselApi] = useState<unknown>(null);

  // Banner images
  const bannerImages = ["/Web_Banner.png", "/banner.png"];

  // Auto play effect
  useEffect(() => {
    if (!carouselApi) return;
    const api = carouselApi as any;
    const interval = setInterval(() => {
      if (api) {
        if (current < bannerImages.length - 1) api.scrollNext();
        else api.scrollTo(0);
      }
    }, 4000);
    return () => clearInterval(interval);
  }, [carouselApi, current, bannerImages.length]);

  // Listen to select event for dots
  useEffect(() => {
    if (!carouselApi) return;
    const api = carouselApi as any;
    const onSelect = () => setCurrent(api.selectedScrollSnap());
    api.on("select", onSelect);
    return () => api.off("select", onSelect);
  }, [carouselApi]);

  return (
    <section className="w-full bg-[#f96d8d] dark:bg-[#2a1a1f] py-0 transition-colors duration-300">
      <div className="relative w-full overflow-hidden shadow-lg">
        <Carousel setApi={setCarouselApi} opts={{ loop: true }}>
          <CarouselContent>
            {bannerImages.map((src, idx) => (
              <CarouselItem key={idx}>
                <Image
                  src={src}
                  alt={`Banner ${idx + 1}`}
                  width={1920}
                  height={500}
                  className="w-full object-cover min-h-[220px] md:min-h-[400px] lg:min-h-[500px]"
                  priority={idx === 0}
                />
              </CarouselItem>
            ))}
          </CarouselContent>
          <CarouselPrevious className="!left-2 !top-1/2 !-translate-y-1/2 z-20 bg-white/80 dark:bg-black/60 hover:bg-white dark:hover:bg-black border-none" />
          <CarouselNext className="!right-2 !top-1/2 !-translate-y-1/2 z-20 bg-white/80 dark:bg-black/60 hover:bg-white dark:hover:bg-black border-none" />
        </Carousel>
        {/* Dots */}
        <div className="absolute left-1/2 -translate-x-1/2 bottom-4 flex gap-2 z-10">
          {bannerImages.map((_, idx) => (
            <button
              key={idx}
              className={`w-3 h-3 rounded-full border-2 ${
                current === idx
                  ? "bg-white border-black dark:bg-[#f96d8d] dark:border-white"
                  : "bg-transparent border-white dark:border-[#f96d8d]"
              }`}
              onClick={() => (carouselApi as any)?.scrollTo(idx)}
              aria-label={`Go to banner ${idx + 1}`}
            />
          ))}
        </div>
      </div>
    </section>
  );
}
