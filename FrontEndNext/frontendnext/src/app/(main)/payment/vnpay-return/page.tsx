"use client";

import { useEffect, useState } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import { useLazyProcessVnPayReturnQuery } from "@/lib/services/vnpayService";

export default function VnPayReturnPage() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const [trigger, { data, isFetching, isError }] =
    useLazyProcessVnPayReturnQuery();
  const [started, setStarted] = useState(false);

  // Important: pass the raw query string to preserve original encoding/casing
  useEffect(() => {
    if (!started) {
      const raw = window.location.search.replace(/^\?/, "");
      if (raw) {
        setStarted(true);
        trigger(raw);
      }
    }
  }, [started, trigger]);

  if (isFetching || !started) {
    return (
      <div className="flex items-center justify-center min-h-[60vh]">
        <div className="text-center">
          <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-primary mx-auto" />
          <p className="mt-4">Đang xử lý kết quả thanh toán...</p>
        </div>
      </div>
    );
  }

  if (isError || !data) {
    return (
      <div className="flex items-center justify-center min-h-[60vh]">
        <div className="text-center">
          <h2 className="text-2xl font-semibold mb-2">Thanh toán thất bại</h2>
          <p className="text-muted-foreground mb-4">
            Có lỗi xảy ra khi xử lý thanh toán
          </p>
          <button
            onClick={() => router.push("/")}
            className="px-4 py-2 bg-primary text-white rounded"
          >
            Quay lại trang chủ
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="container max-w-xl mx-auto p-6">
      <div className="rounded-lg border p-6">
        {data.success ? (
          <div className="text-center">
            <div className="w-16 h-16 bg-green-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg
                className="w-8 h-8 text-green-500"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M5 13l4 4L19 7"
                />
              </svg>
            </div>
            <h2 className="text-2xl font-bold text-green-600 mb-2">
              Thanh toán thành công!
            </h2>
            <p className="text-muted-foreground mb-4">{data.message}</p>
            <div className="text-sm text-muted-foreground space-y-1">
              <p>Mã giao dịch: {data.vnPayTransactionNo}</p>
              <p>Số tiền: {data.amount.toLocaleString("vi-VN")} VNĐ</p>
              <p>Ngân hàng: {data.bankCode}</p>
            </div>
            <button
              onClick={() => router.push(`/orders/${data.orderId}`)}
              className="mt-6 px-4 py-2 bg-primary text-white rounded"
            >
              Xem đơn hàng
            </button>
          </div>
        ) : (
          <div className="text-center">
            <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
              <svg
                className="w-8 h-8 text-red-500"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M6 18L18 6M6 6l12 12"
                />
              </svg>
            </div>
            <h2 className="text-2xl font-bold text-red-600 mb-2">
              Thanh toán thất bại!
            </h2>
            <p className="text-muted-foreground mb-4">{data.message}</p>
            <button
              onClick={() => router.push(`/orders/${data.orderId || ""}`)}
              className="mt-6 px-4 py-2 bg-primary text-white rounded"
            >
              Quay lại đơn hàng
            </button>
          </div>
        )}
      </div>
    </div>
  );
}
