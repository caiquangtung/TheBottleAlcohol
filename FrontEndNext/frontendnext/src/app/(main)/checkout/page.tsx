"use client";

import { useAppSelector } from "@/lib/store/hooks";
import { useCreateOrderMutation } from "@/lib/services/orderService";
import { useCreateVnPayPaymentMutation } from "@/lib/services/vnpayService";
import { CartDetail } from "@/lib/types/cart";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectTrigger,
  SelectContent,
  SelectItem,
  SelectValue,
} from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { toast } from "sonner";

export default function CheckoutPage() {
  const router = useRouter();
  const user = useAppSelector((s) => s.auth.user);
  const cartDetails = useAppSelector(
    (s: { cart: { cartDetails: CartDetail[] } }) => s.cart.cartDetails
  );
  const [createOrder, { isLoading }] = useCreateOrderMutation();
  const [createVnpay] = useCreateVnPayPaymentMutation();

  const [shippingAddress, setShippingAddress] = useState(user?.address || "");
  const [note, setNote] = useState("");
  const [paymentMethod, setPaymentMethod] = useState("VnPay");
  const [shippingMethod, setShippingMethod] = useState("Standard");

  const handlePlaceOrder = async () => {
    if (!user?.id) {
      toast.error("Please log in to place order.");
      return;
    }
    if (!cartDetails || cartDetails.length === 0) {
      toast.error("Your cart is empty.");
      return;
    }
    if (!shippingAddress) {
      toast.error("Please enter shipping address.");
      return;
    }

    try {
      const order = await createOrder({
        customerId: user.id,
        paymentMethod,
        shippingMethod,
        shippingAddress,
        note: note.trim() || undefined,
        orderDetails: cartDetails.map((i) => ({
          productId: i.productId,
          quantity: i.quantity || 1,
        })),
      }).unwrap();
      if (paymentMethod === "VnPay") {
        try {
          const resp = await createVnpay({
            orderId: order.id,
            accountId: user.id,
            amount: order.totalAmount,
            orderDescription: `Thanh toan don hang ${order.id}`,
          }).unwrap();
          window.location.href = resp.paymentUrl;
          return;
        } catch (e) {
          // Fallback: go to order page if VNPAY init fails
          toast.error(
            "Không tạo được thanh toán VNPAY. Chuyển đến trang đơn hàng."
          );
        }
      }
      toast.success("Order created.");
      router.push(`/orders/${order.id}`);
    } catch (e: any) {
      toast.error(e?.data?.message || "Failed to place order.");
    }
  };

  const totalAmount = (cartDetails || []).reduce(
    (s, i) => s + (i.price || 0) * (i.quantity || 0),
    0
  );

  // Check if cart is empty
  if (!cartDetails || cartDetails.length === 0) {
    return (
      <div className="container mx-auto p-6">
        <h1 className="text-2xl font-semibold mb-6">Checkout</h1>
        <div className="text-center py-12">
          <div className="text-6xl mb-4">🛒</div>
          <h2 className="text-xl font-medium mb-2">Giỏ hàng trống</h2>
          <p className="text-muted-foreground mb-6">
            Bạn cần thêm sản phẩm vào giỏ hàng trước khi thanh toán.
          </p>
          <Button onClick={() => router.push("/")}>Tiếp tục mua sắm</Button>
        </div>
      </div>
    );
  }

  return (
    <div className="container mx-auto p-6">
      <h1 className="text-2xl font-semibold mb-6">Checkout</h1>
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="md:col-span-2 space-y-4">
          <div className="rounded border p-4">
            <h2 className="font-medium mb-4">Thông tin giao hàng</h2>
            <div className="space-y-4">
              <div>
                <Label htmlFor="shipping-address">Địa chỉ giao hàng *</Label>
                <textarea
                  id="shipping-address"
                  className="w-full border rounded p-3 min-h-[100px] mt-1"
                  value={shippingAddress}
                  onChange={(e) => setShippingAddress(e.target.value)}
                  placeholder="Nhập địa chỉ giao hàng đầy đủ..."
                />
                {user?.address && (
                  <div className="mt-2">
                    {shippingAddress !== user?.address ? (
                      <button
                        type="button"
                        onClick={() => setShippingAddress(user?.address || "")}
                        className="inline-flex items-center gap-1 text-sm text-blue-600 hover:text-blue-800 hover:underline transition-colors"
                      >
                        📍 Sử dụng địa chỉ từ tài khoản
                      </button>
                    ) : (
                      <div className="text-sm text-green-600 flex items-center gap-1">
                        ✅ Đang sử dụng địa chỉ từ tài khoản
                      </div>
                    )}
                  </div>
                )}
              </div>

              <div>
                <Label htmlFor="order-note">Ghi chú đơn hàng</Label>
                <textarea
                  id="order-note"
                  className="w-full border rounded p-3 min-h-[80px] mt-1"
                  value={note}
                  onChange={(e) => setNote(e.target.value)}
                  placeholder="Ghi chú cho đơn hàng (tùy chọn)..."
                />
              </div>
            </div>
          </div>

          <div className="rounded border p-4">
            <h2 className="font-medium mb-2">Order Items</h2>
            <div className="space-y-2 text-sm">
              {(cartDetails || []).map((i) => (
                <div key={i.productId} className="flex justify-between">
                  <div>
                    <div className="font-medium">{i.productName}</div>
                    <div className="text-muted-foreground">
                      {i.quantity} x {(i.price || 0).toLocaleString("vi-VN")}{" "}
                      VNĐ
                    </div>
                  </div>
                  <div className="font-semibold">
                    {((i.price || 0) * (i.quantity || 0)).toLocaleString(
                      "vi-VN"
                    )}{" "}
                    VNĐ
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
        <div className="md:col-span-1 space-y-4">
          <div className="rounded border p-4">
            <h2 className="font-medium mb-4">Summary</h2>
            <div className="space-y-4 mb-4">
              <div className="space-y-2">
                <Label>Payment Method</Label>
                <Select value={paymentMethod} onValueChange={setPaymentMethod}>
                  <SelectTrigger className="w-full">
                    <SelectValue placeholder="Select payment method" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="VnPay">VNPAY</SelectItem>
                    <SelectItem value="Cash">Cash</SelectItem>
                    <SelectItem value="CreditCard">Credit Card</SelectItem>
                    <SelectItem value="DebitCard">Debit Card</SelectItem>
                    <SelectItem value="BankTransfer">Bank Transfer</SelectItem>
                    <SelectItem value="EWallet">E-Wallet</SelectItem>
                  </SelectContent>
                </Select>
              </div>
              <div className="space-y-2">
                <Label>Shipping Method</Label>
                <Select
                  value={shippingMethod}
                  onValueChange={setShippingMethod}
                >
                  <SelectTrigger className="w-full">
                    <SelectValue placeholder="Select shipping method" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="Standard">Standard</SelectItem>
                    <SelectItem value="Express">Express</SelectItem>
                    <SelectItem value="NextDay">Next Day</SelectItem>
                    <SelectItem value="Pickup">Pickup</SelectItem>
                  </SelectContent>
                </Select>
              </div>
            </div>
            <div className="flex items-center justify-between">
              <span>Total</span>
              <span className="text-lg font-semibold">
                {totalAmount.toLocaleString("vi-VN")} VNĐ
              </span>
            </div>
            <Button
              className="mt-4 w-full"
              onClick={handlePlaceOrder}
              disabled={isLoading}
            >
              {isLoading ? "Placing..." : "Place Order"}
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
