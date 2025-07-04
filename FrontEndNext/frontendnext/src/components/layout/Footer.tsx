import Link from "next/link";
import Image from "next/image";

export default function Footer() {
  return (
    <footer className="bg-[#18181b] text-gray-300 border-t mt-auto pt-10 pb-6">
      <div className="container mx-auto px-4 grid grid-cols-1 md:grid-cols-4 gap-8">
        {/* Cột 1: Logo & liên hệ */}
        <div className="flex flex-col items-start gap-4">
          <div className="flex items-center gap-2 mb-2">
            <Image
              src="/Logo.png"
              alt="Logo"
              width={200}
              height={200}
              className="transition-all filter brightness-0 invert"
            />
          </div>
          <div className="text-sm">
            Hotline:{" "}
            <a href="tel:+84123456789" className="hover:underline">
              +84 123 456 789
            </a>
          </div>
          <div className="flex gap-3 mt-2">
            <a href="#" aria-label="Facebook">
              <i className="fab fa-facebook-f"></i>
            </a>
            <a href="#" aria-label="Instagram">
              <i className="fab fa-instagram"></i>
            </a>
            <a href="#" aria-label="YouTube">
              <i className="fab fa-youtube"></i>
            </a>
            <a href="#" aria-label="Twitter">
              <i className="fab fa-twitter"></i>
            </a>
          </div>
        </div>
        {/* Cột 2: Help & Info */}
        <div>
          <h4 className="font-semibold mb-3 text-white">Help & Information</h4>
          <ul className="space-y-2 text-sm">
            <li>
              <Link href="#" className="hover:underline">
                Why Alcohol Web?
              </Link>
            </li>
            <li>
              <Link href="#" className="hover:underline">
                Sustainability
              </Link>
            </li>
            <li>
              <Link href="#" className="hover:underline">
                Help Centre
              </Link>
            </li>
          </ul>
        </div>
        {/* Cột 3: Useful Links */}
        <div>
          <h4 className="font-semibold mb-3 text-white">Useful Links</h4>
          <ul className="space-y-2 text-sm">
            <li>
              <Link href="#" className="hover:underline">
                Download the app
              </Link>
            </li>
            <li>
              <Link href="#" className="hover:underline">
                Brands A-Z
              </Link>
            </li>
            <li>
              <Link href="#" className="hover:underline">
                Categories A-Z
              </Link>
            </li>
            <li>
              <Link href="/recipe" className="hover:underline">
                Recipe Library
              </Link>
            </li>
            <li>
              <Link href="#" className="hover:underline">
                The Drinks Journal
              </Link>
            </li>
          </ul>
        </div>
        {/* Cột 4: Policies */}
        <div>
          <h4 className="font-semibold mb-3 text-white">Policies</h4>
          <ul className="space-y-2 text-sm">
            <li>
              <Link href="#" className="hover:underline">
                Terms & Conditions
              </Link>
            </li>
            <li>
              <Link href="#" className="hover:underline">
                Delivery & Returns
              </Link>
            </li>
            <li>
              <Link href="#" className="hover:underline">
                Cookie Policy
              </Link>
            </li>
            <li>
              <Link href="#" className="hover:underline">
                Privacy Policy
              </Link>
            </li>
          </ul>
        </div>
      </div>
      <div className="container mx-auto px-4 mt-8 border-t border-gray-700 pt-4 text-center text-gray-500 text-xs">
        © 2024 Alcohol Web. All rights reserved.
      </div>
    </footer>
  );
}
