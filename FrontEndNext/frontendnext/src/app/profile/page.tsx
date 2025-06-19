"use client";
import Header from "../../components/layout/Header";
import ProfilePage from "../../components/ProfilePage";
export default function Account() {
  return (
    <div className="min-h-screen flex flex-col bg-background text-foreground transition-colors duration-300">
      <ProfilePage />
    </div>
  );
}
 