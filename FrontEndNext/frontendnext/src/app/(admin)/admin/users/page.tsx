"use client";
import { Card } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
// import { useGetUsersQuery } from "@/lib/services/userService"; // TODO: Tạo service nếu backend đã có
import { useState } from "react";

export default function AdminUsersPage() {
  // const { data: users, isLoading, error } = useGetUsersQuery();
  const [showAdd, setShowAdd] = useState(false);

  // Placeholder data
  const users = [
    { id: 1, fullName: "Nguyen Van A", email: "a@email.com", role: "admin" },
    { id: 2, fullName: "Tran Thi B", email: "b@email.com", role: "user" },
  ];

  return (
    <Card className="p-6">
      <div className="flex items-center justify-between mb-4">
        <div className="font-bold text-lg">Quản lý người dùng</div>
        <Button onClick={() => setShowAdd(true)}>+ Thêm người dùng</Button>
      </div>
      {/* Table Users */}
      <div className="overflow-x-auto">
        <table className="min-w-full border text-sm">
          <thead>
            <tr className="bg-muted">
              <th className="px-4 py-2 border">ID</th>
              <th className="px-4 py-2 border">Họ tên</th>
              <th className="px-4 py-2 border">Email</th>
              <th className="px-4 py-2 border">Role</th>
              <th className="px-4 py-2 border">Hành động</th>
            </tr>
          </thead>
          <tbody>
            {users.map((user) => (
              <tr key={user.id} className="even:bg-muted/30">
                <td className="px-4 py-2 border">{user.id}</td>
                <td className="px-4 py-2 border">{user.fullName}</td>
                <td className="px-4 py-2 border">{user.email}</td>
                <td className="px-4 py-2 border">{user.role}</td>
                <td className="px-4 py-2 border">
                  <Button size="sm" variant="outline" className="mr-2">
                    Sửa
                  </Button>
                  <Button size="sm" variant="destructive">
                    Xóa
                  </Button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
      {/* TODO: Modal thêm/sửa user, loading, error, toast... */}
      {showAdd && (
        <div className="fixed inset-0 bg-black/30 flex items-center justify-center z-50">
          <div className="bg-white rounded shadow p-6 min-w-[300px]">
            <div className="font-semibold mb-2">Thêm người dùng (demo)</div>
            <Button onClick={() => setShowAdd(false)} variant="outline">
              Đóng
            </Button>
          </div>
        </div>
      )}
    </Card>
  );
}
