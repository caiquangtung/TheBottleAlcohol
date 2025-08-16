"use client";
import Link from "next/link";
import { Button } from "./ui/button";
import { useState } from "react";
import { useGetProfileQuery, useLogoutMutation } from "../lib/services/auth";
import {
  useGetUserOrdersQuery,
  OrderFilter,
} from "../lib/services/orderService";
import { useRouter } from "next/navigation";
import { useDispatch } from "react-redux";
import { logout } from "../lib/features/auth/authSlice";
import { OrderStatusBadge } from "./OrderStatusBadge";

export default function ProfilePage() {
  const router = useRouter();
  const dispatch = useDispatch();
  const { data, isLoading, isError } = useGetProfileQuery();
  const [logoutMutation] = useLogoutMutation();
  const [tab, setTab] = useState<"overview" | "myprofile">("overview");

  // Get user orders for most recent order display
  const { data: ordersData } = useGetUserOrdersQuery(
    {
      customerId: data?.data?.id || 0,
      filter: {
        pageNumber: 1,
        pageSize: 1,
        sortBy: "CreatedAt",
        sortOrder: "desc",
      } as OrderFilter,
    },
    {
      skip: !data?.data?.id,
    }
  );

  // Đảm bảo chỉ render sau khi đã hydrate (nếu cần)
  // useEffect(() => setIsHydrated(true), []);

  const handleLogout = async () => {
    try {
      // Gọi backend logout endpoint để revoke refresh token
      await logoutMutation().unwrap();

      // Xóa local state
      dispatch(logout());

      // Chuyển hướng về login
      router.push("/login");
    } catch (error) {
      console.error("Logout failed:", error);
      // Vẫn logout locally ngay cả khi backend call thất bại
      dispatch(logout());
      router.push("/login");
    }
  };

  if (isLoading) return null;
  if (isError || !data?.success) {
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

  const user = data.data;
  const mostRecentOrder = ordersData?.items?.[0]; // Assuming orders are sorted by date desc

  return (
    <div className="container mx-auto py-10 flex flex-col md:flex-row gap-8">
      {/* Sidebar */}
      <aside className="md:w-1/4 border-r pr-8">
        <h2 className="text-2xl font-bold mb-6">MY ACCOUNT</h2>
        <div className="mb-4 font-semibold">Welcome back, {user.fullName}</div>
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
            href="/orders"
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
          <button
            onClick={handleLogout}
            className="flex items-center justify-between py-2 px-1 border-b border-transparent hover:border-black transition-all"
          >
            Logout <span>&#8250;</span>
          </button>
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
                {mostRecentOrder ? (
                  <>
                    <div className="flex items-center justify-between mb-2">
                      <div className="font-semibold">
                        Đơn hàng #{mostRecentOrder.id}
                      </div>
                      <OrderStatusBadge status={mostRecentOrder.status} />
                    </div>
                    <div className="text-sm text-muted-foreground mb-2">
                      Ngày đặt:{" "}
                      {new Date(mostRecentOrder.createdAt).toLocaleDateString(
                        "vi-VN"
                      )}
                    </div>
                    <div className="text-sm mb-2">
                      Tổng tiền:{" "}
                      <span className="font-semibold">
                        {mostRecentOrder.totalAmount.toLocaleString("vi-VN")}{" "}
                        VNĐ
                      </span>
                    </div>
                    <div className="text-sm mb-4">
                      {mostRecentOrder.orderDetails?.length || 0} sản phẩm
                    </div>
                    <div className="flex gap-2">
                      <Link href={`/orders/${mostRecentOrder.id}`}>
                        <Button variant="outline" size="sm">
                          XEM CHI TIẾT
                        </Button>
                      </Link>
                      <Link href="/orders">
                        <Button variant="outline" size="sm">
                          TẤT CẢ ĐƠN HÀNG
                        </Button>
                      </Link>
                    </div>
                  </>
                ) : (
                  <>
                    <div className="font-semibold mb-2">No orders yet...</div>
                    <div className="text-sm mb-4">
                      You haven&apos;t placed any orders yet.
                    </div>
                    <Link href="/">
                      <Button variant="outline">SHOP NOW</Button>
                    </Link>
                  </>
                )}
              </div>
            </div>
            {/* My Details & Default Address */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
              {/* My Details */}
              <div>
                <h4 className="text-lg font-bold mb-2">MY DETAILS</h4>
                <div className="mb-1">{user.fullName}</div>
                <div className="mb-1">{user.email}</div>
                <Link href="#" className="underline text-sm">
                  View All Details
                </Link>
              </div>
              {/* Default Address */}
              <div>
                <h4 className="text-lg font-bold mb-4">DEFAULT ADDRESS</h4>
                <div className="mb-1">{user.fullName}</div>
                {user.address &&
                  user.address.split(",").map((line: string, idx: number) => (
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
            <div className="mb-2">Họ tên: {user.fullName}</div>
            <div className="mb-2">Email: {user.email}</div>
            <div className="mb-2">Địa chỉ: {user.address}</div>
            <div className="mb-2">SĐT: {user.phoneNumber}</div>
            <div className="mb-2">Ngày sinh: {user.dateOfBirth}</div>
            <div className="mb-2">Giới tính: {user.gender}</div>
            <div className="mb-2">Vai trò: {user.role}</div>
            <div className="mb-2">
              Trạng thái: {user.status ? "Active" : "Inactive"}
            </div>
            <div className="mb-2">Ngày tạo: {user.createdAt}</div>
          </section>
        )}
      </main>
    </div>
  );
}
