"use client";
import Image from "next/image";
import { useState } from "react";
import { useRouter } from "next/navigation";
import { Input } from "./ui/input";
import { Button } from "./ui/button";
import { toast } from "sonner";
import Link from "next/link";
import { useRegisterMutation } from "../lib/services/auth";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";

type Gender = "Male" | "Female" | "Other";

interface FormData {
  fullName: string;
  email: string;
  password: string;
  confirmPassword: string;
  phoneNumber: string;
  address: string;
  dateOfBirth: string;
  gender: Gender;
}

export default function RegisterPage() {
  const router = useRouter();
  const [register, { isLoading }] = useRegisterMutation();
  const [formData, setFormData] = useState<FormData>({
    fullName: "",
    email: "",
    password: "",
    confirmPassword: "",
    phoneNumber: "",
    address: "",
    dateOfBirth: "",
    gender: "Male",
  });

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    // Validate password match
    if (formData.password !== formData.confirmPassword) {
      toast.error("Mật khẩu không khớp", {
        description: "Vui lòng kiểm tra lại mật khẩu xác nhận",
      });
      return;
    }

    try {
      const response = await register({
        fullName: formData.fullName,
        email: formData.email,
        password: formData.password,
        phoneNumber: formData.phoneNumber,
        address: formData.address,
        dateOfBirth: formData.dateOfBirth,
        gender: formData.gender,
      }).unwrap();

      if (response.success) {
        toast.success("Đăng ký thành công!", {
          description: "Vui lòng đăng nhập để tiếp tục",
        });

        // Chuyển hướng đến trang đăng nhập sau 1 giây
        setTimeout(() => {
          router.push("/login");
        }, 1000);
      } else {
        toast.error("Đăng ký thất bại", {
          description: response.message || "Vui lòng kiểm tra lại thông tin",
        });
      }
    } catch (err) {
      console.error("Register error:", err);
      toast.error("Đăng ký thất bại", {
        description: "Có lỗi xảy ra, vui lòng thử lại sau",
      });
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const handleSelectChange = (name: string, value: Gender) => {
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  return (
    <div className="min-h-screen flex flex-col md:flex-row bg-background">
      {/* Left: Image */}
      <div className="hidden md:block md:w-1/2 h-[320px] md:h-auto relative">
        <Image
          src="/login-side.jpg"
          alt="Register visual"
          fill
          sizes="(max-width: 768px) 100vw, 50vw"
          className="object-cover object-center w-full h-full"
          priority
        />
        <div className="absolute inset-0 bg-black/40" />
      </div>
      {/* Right: Register Form */}
      <div className="flex-1 flex items-center justify-center bg-white dark:bg-[#18181b]">
        <div className="w-full max-w-md p-8">
          <h2 className="text-2xl md:text-3xl font-bold text-center mb-6">
            CREATE AN ACCOUNT
          </h2>
          <form className="flex flex-col gap-4" onSubmit={handleSubmit}>
            <Input
              type="text"
              name="fullName"
              placeholder="Full name"
              value={formData.fullName}
              onChange={handleChange}
              required
            />
            <Input
              type="email"
              name="email"
              placeholder="Email address"
              value={formData.email}
              onChange={handleChange}
              required
            />
            <Input
              type="tel"
              name="phoneNumber"
              placeholder="Phone number"
              value={formData.phoneNumber}
              onChange={handleChange}
              required
            />
            <Input
              type="text"
              name="address"
              placeholder="Address"
              value={formData.address}
              onChange={handleChange}
              required
            />
            <Input
              type="date"
              name="dateOfBirth"
              placeholder="Date of birth"
              value={formData.dateOfBirth}
              onChange={handleChange}
              required
            />
            <Select
              value={formData.gender}
              onValueChange={(value: Gender) =>
                handleSelectChange("gender", value)
              }
            >
              <SelectTrigger>
                <SelectValue placeholder="Select gender" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="Male">Male</SelectItem>
                <SelectItem value="Female">Female</SelectItem>
                <SelectItem value="Other">Other</SelectItem>
              </SelectContent>
            </Select>
            <Input
              type="password"
              name="password"
              placeholder="Password"
              value={formData.password}
              onChange={handleChange}
              required
            />
            <Input
              type="password"
              name="confirmPassword"
              placeholder="Confirm password"
              value={formData.confirmPassword}
              onChange={handleChange}
              required
            />
            <Button
              type="submit"
              className="w-full font-bold text-base bg-black dark:bg-white dark:text-black text-white hover:bg-neutral-800 dark:hover:bg-neutral-200"
              disabled={isLoading}
            >
              {isLoading ? "CREATING ACCOUNT..." : "CREATE ACCOUNT"}
            </Button>
          </form>
          <div className="my-6 border-t" />
          <div className="text-center text-sm mb-2">
            Already have an account?
          </div>
          <Link href="/login">
            <Button variant="outline" className="w-full font-bold text-base">
              LOGIN
            </Button>
          </Link>
        </div>
      </div>
    </div>
  );
}
