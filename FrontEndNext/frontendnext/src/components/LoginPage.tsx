"use client";
import Image from "next/image";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { useDispatch } from "react-redux";
import { loginSuccess } from "../lib/features/auth/authSlice";
import { useLoginMutation } from "../lib/services/auth";
import { Input } from "./ui/input";
import { Button } from "./ui/button";
import { toast } from "sonner";
import Link from "next/link";

export default function LoginPage() {
  const router = useRouter();
  const dispatch = useDispatch();
  const [login, { isLoading }] = useLoginMutation();

  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      const response = await login(formData).unwrap();
      if (response.success) {
        // Lưu thông tin user vào localStorage (không lưu accessToken)
        const userData = {
          id: response.data.id,
          fullName: response.data.fullName,
          email: response.data.email,
          role: response.data.role,
        };
        localStorage.setItem("user", JSON.stringify(userData));
        // Không lưu accessToken vào localStorage nữa

        // Dispatch action để cập nhật Redux store
        dispatch(
          loginSuccess({
            user: userData,
            accessToken: response.data.accessToken,
            // Không cần refreshToken - backend xử lý qua HttpOnly cookie
          })
        );

        // Hiển thị thông báo thành công
        toast.success(response.message || "Đăng nhập thành công!", {
          description: "Chào mừng bạn quay trở lại!",
        });

        // Chuyển hướng sau 1 giây
        setTimeout(() => {
          router.push("/");
        }, 1000);
      } else {
        toast.error("Đăng nhập thất bại", {
          description:
            response.message || "Vui lòng kiểm tra lại thông tin đăng nhập",
        });
      }
    } catch (err) {
      console.error("Login error:", err);
      toast.error("Đăng nhập thất bại", {
        description: "Email hoặc mật khẩu không đúng",
      });
    }
  };

  return (
    <div className="min-h-screen flex flex-col md:flex-row bg-background">
      {/* Left: Image */}
      <div className="hidden md:block md:w-1/2 h-[320px] md:h-auto relative">
        <Image
          src="/login-side.jpg"
          alt="Login visual"
          fill
          sizes="(max-width: 768px) 100vw, 50vw"
          className="object-cover object-center w-full h-full"
          priority
        />
        <div className="absolute inset-0 bg-black/40" />
      </div>
      {/* Right: Login Form */}
      <div className="flex-1 flex items-center justify-center bg-white dark:bg-[#18181b]">
        <div className="w-full max-w-md p-8">
          <h2 className="text-2xl md:text-3xl font-bold text-center mb-6">
            LOGIN TO YOUR ACCOUNT
          </h2>
          <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
            <Input
              type="email"
              name="email"
              placeholder="Email address"
              value={formData.email}
              onChange={handleInputChange}
              required
            />
            <Input
              type="password"
              name="password"
              placeholder="Password"
              value={formData.password}
              onChange={handleInputChange}
              required
            />
            <Button
              type="submit"
              className="w-full font-bold text-base bg-black dark:bg-white dark:text-black text-white hover:bg-neutral-800 dark:hover:bg-neutral-200"
              disabled={isLoading}
            >
              {isLoading ? "LOGGING IN..." : "LOGIN"}
            </Button>
          </form>
          <div className="flex flex-col items-center gap-2 mt-4">
            <a href="#" className="text-xs underline text-muted-foreground">
              Forgot password
            </a>
            <a href="#" className="text-xs underline text-muted-foreground">
              Trade Login
            </a>
          </div>
          <div className="my-6 border-t" />
          <div className="text-center text-sm mb-2">Not signed up yet?</div>
          <Link href="/register">
            <Button variant="outline" className="w-full font-bold text-base">
              SIGN UP
            </Button>
          </Link>
        </div>
      </div>
    </div>
  );
}
