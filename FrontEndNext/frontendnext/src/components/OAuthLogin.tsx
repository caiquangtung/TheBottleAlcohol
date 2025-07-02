"use client";
import React from "react";

export default function OAuthLogin() {
  const handleGoogleLogin = () => {
    window.location.href = "http://localhost:8080/api/v1/oauth/login/google";
  };

  const handleFacebookLogin = () => {
    window.location.href = "http://localhost:8080/api/v1/oauth/login/facebook";
  };

  return (
    <div className="flex flex-col gap-3 mt-4">
      <button
        onClick={handleGoogleLogin}
        className="w-full flex items-center justify-center gap-2 bg-white border border-gray-300 text-gray-800 font-semibold py-2 rounded hover:bg-gray-100"
      >
        <img src="/google.svg" alt="Google" className="w-5 h-5" />
        Đăng nhập với Google
      </button>
      <button
        onClick={handleFacebookLogin}
        className="w-full flex items-center justify-center gap-2 bg-blue-600 text-white font-semibold py-2 rounded hover:bg-blue-700"
      >
        <img src="/facebook.svg" alt="Facebook" className="w-5 h-5" />
        Đăng nhập với Facebook
      </button>
    </div>
  );
}
