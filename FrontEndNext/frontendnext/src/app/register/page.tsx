"use client";
import Header from "../../components/layout/Header";
import RegisterPage from "../../components/RegisterPage";

export default function Register() {
  return (
    <div className="min-h-screen flex flex-col bg-background text-foreground transition-colors duration-300">
      <RegisterPage />
    </div>
  );
}
