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
} from "lucide-react";

const menu = [
  { label: "Dashboard", href: "/admin", icon: LayoutDashboard },
  { label: "Users", href: "/admin/users", icon: Users },
  { label: "Products", href: "/admin/products", icon: Package },
  { label: "Categories", href: "/admin/categories", icon: FolderOpen },
  { label: "Brands", href: "/admin/brands", icon: Tag },
  { label: "Suppliers", href: "/admin/suppliers", icon: Truck },
  { label: "Discounts", href: "/admin/discounts", icon: Percent },
  { label: "Inventory", href: "/admin/inventory", icon: Warehouse },
  { label: "Recipes", href: "/admin/recipes", icon: ChefHat },
  { label: "Reviews", href: "/admin/reviews", icon: Star },
  { label: "Notifications", href: "/admin/notifications", icon: Bell },
  { label: "Orders", href: "/admin/orders", icon: ShoppingCart },
];

function AdminLayoutContent({ children }: { children: ReactNode }) {
  const pathname = usePathname();

  return (
    <div className="flex min-h-screen bg-gray-50">
      {/* Sidebar */}
      <aside className="w-64 bg-white border-r border-gray-200 flex flex-col shadow-sm">
        <div className="h-16 flex items-center justify-center font-bold text-lg border-b border-gray-200 bg-gradient-to-r from-blue-600 to-blue-700 text-white">
          <Package className="h-6 w-6 mr-2" />
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
                        ? "bg-blue-50 text-blue-700 border-r-2 border-blue-700"
                        : "text-gray-700 hover:bg-gray-100 hover:text-gray-900"
                    }`}
                  >
                    <Icon
                      className={`h-4 w-4 mr-3 ${
                        isActive ? "text-blue-700" : "text-gray-500"
                      }`}
                    />
                    {item.label}
                  </Link>
                </li>
              );
            })}
          </ul>
        </nav>

        <div className="p-4 border-t border-gray-200">
          <div className="flex items-center space-x-3 mb-3">
            <Avatar className="w-8 h-8">
              <div className="bg-blue-100 text-blue-700 rounded-full w-8 h-8 flex items-center justify-center text-sm font-medium">
                A
              </div>
            </Avatar>
            <div className="flex-1 min-w-0">
              <p className="text-sm font-medium text-gray-900 truncate">
                Admin User
              </p>
              <p className="text-xs text-gray-500 truncate">
                admin@example.com
              </p>
            </div>
          </div>
          <div className="space-y-1">
            <Button
              variant="ghost"
              size="sm"
              className="w-full justify-start text-gray-700 hover:text-gray-900"
            >
              <Settings className="h-4 w-4 mr-2" />
              Settings
            </Button>
            <Button
              variant="ghost"
              size="sm"
              className="w-full justify-start text-red-600 hover:text-red-700 hover:bg-red-50"
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
        <header className="h-16 bg-white border-b border-gray-200 flex items-center justify-between px-6 shadow-sm">
          <div className="flex items-center space-x-4">
            <h1 className="text-xl font-semibold text-gray-900">
              {menu.find((item) => item.href === pathname)?.label ||
                "Dashboard"}
            </h1>
          </div>
          <div className="flex items-center space-x-4">
            <div className="flex items-center space-x-2 text-sm text-gray-600">
              <span>Welcome back,</span>
              <span className="font-medium text-gray-900">Admin</span>
            </div>
            <div className="w-px h-6 bg-gray-300"></div>
            <Button variant="outline" size="sm" className="text-gray-700">
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
        <footer className="h-12 flex items-center justify-center text-xs text-gray-500 border-t border-gray-200 bg-white">
          <div className="flex items-center space-x-2">
            <Package className="h-3 w-3" />
            <span>Alcohol Admin &copy; 2024</span>
          </div>
        </footer>
      </div>
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
