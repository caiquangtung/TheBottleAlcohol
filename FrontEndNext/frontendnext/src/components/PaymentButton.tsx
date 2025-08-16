"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { useCreateVnPayPaymentMutation } from "@/lib/services/vnpayService";

interface PaymentButtonProps {
  orderId: number;
  accountId: number;
  amount: number;
  orderDescription?: string;
}

export function PaymentButton({
  orderId,
  accountId,
  amount,
  orderDescription,
}: PaymentButtonProps) {
  const [isLoading, setIsLoading] = useState(false);
  const [createPayment] = useCreateVnPayPaymentMutation();

  const handlePayment = async () => {
    try {
      setIsLoading(true);
      const res = await createPayment({
        orderId,
        accountId,
        amount,
        orderDescription: orderDescription || `Thanh toan don hang #${orderId}`,
      }).unwrap();
      window.location.href = res.paymentUrl;
    } catch (err) {
      console.error("Create payment failed", err);
      alert("Không thể tạo thanh toán. Vui lòng thử lại.");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <Button onClick={handlePayment} disabled={isLoading} className="w-full">
      {isLoading
        ? "Đang xử lý..."
        : `Thanh toán ${amount.toLocaleString("vi-VN")} VNĐ`}
    </Button>
  );
}
