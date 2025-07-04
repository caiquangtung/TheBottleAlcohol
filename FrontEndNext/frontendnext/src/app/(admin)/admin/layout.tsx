import "@/styles/globals.css";
import { ReactNode } from "react";
import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Avatar } from "@/components/ui/avatar";

const menu = [
  { label: "Dashboard", href: "/admin" },
  { label: "Users", href: "/admin/users" },
  { label: "Products", href: "/admin/products" },
  { label: "Orders", href: "/admin/orders" },
];

export default function AdminLayout({ children }: { children: ReactNode }) {
  return (
    <div className="flex min-h-screen bg-muted">
      {/* Sidebar */}
      <aside className="w-64 bg-white border-r flex flex-col">
        <div className="h-16 flex items-center justify-center font-bold text-lg border-b">
          Admin Panel
        </div>
        <nav className="flex-1 py-4">
          <ul className="space-y-2">
            {menu.map((item) => (
              <li key={item.href}>
                <Link
                  href={item.href}
                  className="block px-6 py-2 rounded hover:bg-muted-foreground/10 transition"
                >
                  {item.label}
                </Link>
              </li>
            ))}
          </ul>
        </nav>
        <div className="p-4 border-t text-xs text-muted-foreground">
          © 2024 Alcohol Admin
        </div>
      </aside>
      {/* Main content */}
      <div className="flex-1 flex flex-col">
        {/* Header */}
        <header className="h-16 bg-white border-b flex items-center justify-between px-6">
          <div className="font-semibold text-lg">Dashboard</div>
          <div className="flex items-center gap-4">
            <span className="text-sm">Admin</span>
            <Avatar className="w-8 h-8" />
            <Button variant="outline" size="sm">
              Logout
            </Button>
          </div>
        </header>
        {/* Content */}
        <main className="flex-1 p-6 overflow-y-auto">{children}</main>
        {/* Footer */}
        <footer className="h-12 flex items-center justify-center text-xs text-muted-foreground border-t bg-white">
          Alcohol Admin &copy; 2024
        </footer>
      </div>
    </div>
  );
}
