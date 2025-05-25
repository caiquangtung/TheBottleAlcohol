"use client";
import Link from "next/link";
import { Button } from "./ui/button";
import { useState } from "react";
import { useGetProfileQuery } from "../lib/services/auth";
import { useRouter } from "next/navigation";

export default function ProfilePage() {
  const router = useRouter();
  const { data, isLoading, isError } = useGetProfileQuery();
  const [isHydrated, setIsHydrated] = useState(false);
  const [tab, setTab] = useState<"overview" | "myprofile">("overview");

  // Đảm bảo chỉ render sau khi đã hydrate (nếu cần)
  // useEffect(() => setIsHydrated(true), []);

  if (isLoading) return null;
  if (isError || !data?.Success) {
    // Chuyển hướng về trang đăng nhập sau 2 giây
    setTimeout(() => {
      router.push("/login");
    }, 2000);
    return (
      <div className="container mx-auto py-10 text-center">
        <div className="text-red-500 mb-4">Không tìm thấy thông tin user.</div>
        <div className="text-gray-600">
          Đang chuyển hướng về trang đăng nhập...
        </div>
      </div>
    );
  }

  const user = data.Data;

  return (
    <div className="container mx-auto py-10 flex flex-col md:flex-row gap-8">
      {/* Sidebar */}
      <aside className="md:w-1/4 border-r pr-8">
        <h2 className="text-2xl font-bold mb-6">MY ACCOUNT</h2>
        <div className="mb-4 font-semibold">Welcome back, {user.FullName}</div>
        <nav className="flex flex-col gap-2">
          <button
            onClick={() => setTab("overview")}
            className={`flex items-center justify-between py-2 px-1 font-semibold border-b border-transparent hover:border-black transition-all ${
              tab === "overview" ? "border-black" : ""
            }`}
          >
            Overview <span>&#8250;</span>
          </button>
          <button
            onClick={() => setTab("myprofile")}
            className={`flex items-center justify-between py-2 px-1 border-b border-transparent hover:border-black transition-all ${
              tab === "myprofile" ? "border-black" : ""
            }`}
          >
            My profile <span>&#8250;</span>
          </button>
          <Link
            href="#"
            className="flex items-center justify-between py-2 px-1 border-b border-transparent hover:border-black transition-all"
          >
            Orders <span>&#8250;</span>
          </Link>
          <Link
            href="#"
            className="flex items-center justify-between py-2 px-1 border-b border-transparent hover:border-black transition-all"
          >
            Addresses <span>&#8250;</span>
          </Link>
          <Link
            href="#"
            className="flex items-center justify-between py-2 px-1 border-b border-transparent hover:border-black transition-all"
          >
            Wishlist <span>&#8250;</span>
          </Link>
          <Link
            href="#"
            className="flex items-center justify-between py-2 px-1 border-b border-transparent hover:border-black transition-all"
          >
            Logout <span>&#8250;</span>
          </Link>
        </nav>
      </aside>
      {/* Main content */}
      <main className="flex-1 grid grid-cols-1 md:grid-cols-2 gap-8">
        {tab === "overview" && (
          <section className="col-span-2 flex flex-col gap-8">
            {/* Most Recent Order */}
            <div>
              <h3 className="text-lg font-bold mb-4">MOST RECENT ORDER</h3>
              <div className="border rounded p-6 mb-6">
                <div className="font-semibold mb-2">No orders yet...</div>
                <div className="text-sm mb-4">
                  You haven't placed any orders yet.
                </div>
                <Button variant="outline">SHOP NOW</Button>
              </div>
            </div>
            {/* My Details & Default Address */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
              {/* My Details */}
              <div>
                <h4 className="text-lg font-bold mb-2">MY DETAILS</h4>
                <div className="mb-1">{user.FullName}</div>
                <div className="mb-1">{user.Email}</div>
                <Link href="#" className="underline text-sm">
                  View All Details
                </Link>
              </div>
              {/* Default Address */}
              <div>
                <h4 className="text-lg font-bold mb-4">DEFAULT ADDRESS</h4>
                <div className="mb-1">{user.FullName}</div>
                {user.Address &&
                  user.Address.split(",").map((line: string, idx: number) => (
                    <div className="mb-1" key={idx}>
                      {line.trim()}
                    </div>
                  ))}
                <Link href="#" className="underline text-sm">
                  View All Addresses
                </Link>
              </div>
            </div>
          </section>
        )}
        {tab === "myprofile" && (
          <section className="col-span-2">
            <h3 className="text-lg font-bold mb-4">MY PROFILE</h3>
            <div className="mb-2">Họ tên: {user.FullName}</div>
            <div className="mb-2">Email: {user.Email}</div>
            <div className="mb-2">Địa chỉ: {user.Address}</div>
            <div className="mb-2">SĐT: {user.PhoneNumber}</div>
            <div className="mb-2">Ngày sinh: {user.DateOfBirth}</div>
            <div className="mb-2">Giới tính: {user.Gender}</div>
            <div className="mb-2">Vai trò: {user.Role}</div>
            <div className="mb-2">
              Trạng thái: {user.Status ? "Active" : "Inactive"}
            </div>
            <div className="mb-2">Ngày tạo: {user.CreatedAt}</div>
          </section>
        )}
      </main>
    </div>
  );
}
