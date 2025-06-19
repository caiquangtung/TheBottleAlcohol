"use client";
import Header from "../../components/layout/Header";
import LoginPage from "../../components/LoginPage";

export default function Login() {
  return (
    <div className="min-h-screen flex flex-col bg-background text-foreground transition-colors duration-300">
      <LoginPage />
    </div>
  );
}
