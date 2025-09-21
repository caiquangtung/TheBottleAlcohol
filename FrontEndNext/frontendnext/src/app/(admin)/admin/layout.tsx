"use client";
import "@/styles/globals.css";
import { ReactNode } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Avatar } from "@/components/ui/avatar";
import { Providers } from "@/lib/store/Providers";
import { Toaster } from "sonner";
import {
  LayoutDashboard,
  Users,
  Package,
  FolderOpen,
  Tag,
  Truck,
  Percent,
  Warehouse,
  ChefHat,
  Star,
  Bell,
  ShoppingCart,
  LogOut,
  Settings,
  PackageOpen,
} from "lucide-react";
import FloatingDarkModeButton from "@/components/FloatingDarkModeButton";

const menu = [
  { label: "Dashboard", href: "/admin", icon: LayoutDashboard },
  { label: "Users", href: "/admin/users", icon: Users },
  { label: "Products", href: "/admin/products", icon: Package },
  { label: "Categories", href: "/admin/categories", icon: FolderOpen },
  { label: "Brands", href: "/admin/brands", icon: Tag },
  { label: "Suppliers", href: "/admin/suppliers", icon: Truck },
  { label: "Discounts", href: "/admin/discounts", icon: Percent },
  { label: "Inventory", href: "/admin/inventory", icon: Warehouse },
  { label: "Import Orders", href: "/admin/import-orders", icon: PackageOpen },
  { label: "Recipes", href: "/admin/recipes", icon: ChefHat },
  { label: "Reviews", href: "/admin/reviews", icon: Star },
  { label: "Notifications", href: "/admin/notifications", icon: Bell },
  { label: "Orders", href: "/admin/orders", icon: ShoppingCart },
];

function AdminLayoutContent({ children }: { children: ReactNode }) {
  const pathname = usePathname();

  return (
    <div className="flex min-h-screen bg-gray-50 dark:bg-gray-900">
      {/* Sidebar */}
      <aside className="w-64 bg-white dark:bg-gray-950 border-r border-gray-200 dark:border-gray-800 flex flex-col shadow-sm">
        <div className="h-16 flex items-center justify-center font-bold text-lg border-b border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-950 text-black dark:text-white">
          <Package className="h-6 w-6 mr-2 text-black dark:text-white" />
          Admin Panel
        </div>

        <nav className="flex-1 py-4 px-3">
          <ul className="space-y-1">
            {menu.map((item) => {
              const Icon = item.icon;
              const isActive = pathname === item.href;

              return (
                <li key={item.href}>
                  <Link
                    href={item.href}
                    className={`flex items-center px-3 py-2 rounded-lg text-sm font-medium transition-colors ${
                      isActive
                        ? "bg-blue-50 dark:bg-blue-950/40 text-blue-700 dark:text-blue-400 border-r-2 border-blue-700 dark:border-blue-400"
                        : "text-gray-700 dark:text-gray-300 hover:bg-gray-100 dark:hover:bg-gray-800 hover:text-gray-900 dark:hover:text-white"
                    }`}
                  >
                    <Icon
                      className={`h-4 w-4 mr-3 ${
                        isActive
                          ? "text-blue-700 dark:text-blue-400"
                          : "text-gray-500 dark:text-gray-400"
                      }`}
                    />
                    {item.label}
                  </Link>
                </li>
              );
            })}
          </ul>
        </nav>

        <div className="p-4 border-t border-gray-200 dark:border-gray-800">
          <div className="flex items-center space-x-3 mb-3">
            <Avatar className="w-8 h-8">
              <div className="bg-gray-200 dark:bg-gray-800 text-gray-800 dark:text-gray-200 rounded-full w-8 h-8 flex items-center justify-center text-sm font-medium">
                A
              </div>
            </Avatar>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-gray-900 dark:text-gray-100 truncate">
                Admin User
              </p>
              <p className="text-xs text-gray-500 dark:text-gray-400 truncate">
                admin@example.com
              </p>
            </div>
          </div>
          <div className="space-y-1">
            <Button
              variant="ghost"
              size="sm"
              className="w-full justify-start text-gray-700 dark:text-gray-300 hover:text-gray-900 dark:hover:text-white"
            >
              <Settings className="h-4 w-4 mr-2" />
              Settings
            </Button>
            <Button
              variant="ghost"
              size="sm"
              className="w-full justify-start text-red-600 hover:text-red-700 hover:bg-red-50 dark:hover:bg-red-950/30"
            >
              <LogOut className="h-4 w-4 mr-2" />
              Logout
            </Button>
          </div>
        </div>
      </aside>

      {/* Main content */}
      <div className="flex-1 flex flex-col">
        {/* Header */}
        <header className="h-16 bg-white dark:bg-gray-950 border-b border-gray-200 dark:border-gray-800 flex items-center justify-between px-6 shadow-sm">
          <div className="flex items-center space-x-4">
            <h1 className="text-xl font-semibold text-gray-900 dark:text-gray-100">
              {menu.find((item) => item.href === pathname)?.label ||
                "Dashboard"}
            </h1>
          </div>
          <div className="flex items-center space-x-4">
            <div className="flex items-center space-x-2 text-sm text-gray-600 dark:text-gray-300">
              <span>Welcome back,</span>
              <span className="font-medium text-gray-900 dark:text-gray-100">
                Admin
              </span>
            </div>
            <div className="w-px h-6 bg-gray-300"></div>
            <Button
              variant="outline"
              size="sm"
              className="text-gray-700 dark:text-gray-200"
            >
              <Bell className="h-4 w-4 mr-2" />
              Notifications
            </Button>
          </div>
        </header>

        {/* Content */}
        <main className="flex-1 p-6 overflow-y-auto">
          <div className="max-w-7xl mx-auto">{children}</div>
        </main>

        {/* Footer */}
        <footer className="h-12 flex items-center justify-center text-xs text-gray-500 dark:text-gray-400 border-t border-gray-200 dark:border-gray-800 bg-white dark:bg-gray-950">
          <div className="flex items-center space-x-2">
            <Package className="h-3 w-3" />
            <span>Alcohol Admin &copy; 2024</span>
          </div>
        </footer>
      </div>
      <FloatingDarkModeButton />
    </div>
  );
}

export default function AdminLayout({ children }: { children: ReactNode }) {
  return (
    <Providers>
      <AdminLayoutContent>{children}</AdminLayoutContent>
      <Toaster position="top-center" richColors />
    </Providers>
  );
}
