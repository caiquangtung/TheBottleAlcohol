"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Badge } from "@/components/ui/badge";
import { Switch } from "@/components/ui/switch";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { toast } from "sonner";
import { Plus, Edit, Trash2, Search } from "lucide-react";
import {
  useGetUsersQuery,
  useCreateUserMutation,
  useUpdateUserMutation,
  useDeleteUserMutation,
} from "@/lib/services/userService";
import type { User, UserCreate, UserUpdate } from "@/lib/types/user";

const defaultForm: UserCreate = {
  fullName: "",
  dateOfBirth: "",
  address: "",
  gender: 0, // Male
  phoneNumber: "",
  email: "",
  password: "",
  role: 1, // Admin
};

export default function AdminUsersPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [editingUser, setEditingUser] = useState<User | null>(null);
  const [deletingId, setDeletingId] = useState<number | null>(null);

  // Queries
  const { data: users = [], isLoading, isError } = useGetUsersQuery();

  // Mutations
  const [createUser, { isLoading: isCreating }] = useCreateUserMutation();
  const [updateUser, { isLoading: isUpdating }] = useUpdateUserMutation();
  const [deleteUser, { isLoading: isDeleting }] = useDeleteUserMutation();

  // Filter users based on search term
  const filteredUsers = users.filter(
    (user) =>
      user.fullName.toLowerCase().includes(searchTerm.toLowerCase()) ||
      user.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleCreateUser = async (formData: UserCreate) => {
    try {
      await createUser(formData).unwrap();
      toast.success("Thêm người dùng thành công!");
      setIsCreateModalOpen(false);
    } catch (error: any) {
      toast.error(error?.data?.message || "Thêm người dùng thất bại!");
    }
  };

  const handleUpdateUser = async (formData: UserUpdate) => {
    if (!editingUser) return;
    try {
      await updateUser({ id: editingUser.id, data: formData }).unwrap();
      toast.success("Cập nhật người dùng thành công!");
      setIsEditModalOpen(false);
      setEditingUser(null);
    } catch (error: any) {
      toast.error(error?.data?.message || "Cập nhật người dùng thất bại!");
    }
  };

  const handleDeleteUser = async (userId: number) => {
    try {
      await deleteUser(userId).unwrap();
      toast.success("Xóa người dùng thành công!");
      setDeletingId(null);
    } catch (error: any) {
      toast.error(error?.data?.message || "Xóa người dùng thất bại!");
    }
  };

  const openEditModal = (user: User) => {
    setEditingUser(user);
    setIsEditModalOpen(true);
  };

  const getRoleLabel = (role: number) => {
    switch (role) {
      case 0:
        return "User";
      case 1:
        return "Admin";
      case 2:
        return "CEO";
      case 3:
        return "Manager";
      default:
        return "Unknown";
    }
  };

  const getGenderLabel = (gender: number) => {
    switch (gender) {
      case 0:
        return "Nam";
      case 1:
        return "Nữ";
      case 2:
        return "Khác";
      default:
        return "Unknown";
    }
  };

  if (isError) {
    return (
      <div className="p-6">
        <Card>
          <CardContent className="pt-6">
            <div className="text-center text-red-600">
              Có lỗi xảy ra khi tải dữ liệu người dùng
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="p-6 space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold">Quản lý người dùng</h1>
        <Dialog open={isCreateModalOpen} onOpenChange={setIsCreateModalOpen}>
          <DialogTrigger asChild>
            <Button>
              <Plus className="w-4 h-4 mr-2" />
              Thêm người dùng
            </Button>
          </DialogTrigger>
          <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
            <DialogHeader>
              <DialogTitle>Thêm người dùng mới</DialogTitle>
            </DialogHeader>
            <UserCreateForm
              onSubmit={handleCreateUser}
              isLoading={isCreating}
            />
          </DialogContent>
        </Dialog>
      </div>

      {/* Search */}
      <Card>
        <CardContent className="pt-6">
          <div className="relative">
            <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Tìm kiếm người dùng..."
              value={searchTerm}
              onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
                setSearchTerm(e.target.value)
              }
              className="pl-10"
            />
          </div>
        </CardContent>
      </Card>

      {/* Users Table */}
      <Card>
        <CardHeader>
          <CardTitle>Người dùng ({filteredUsers.length})</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="text-center py-8">Đang tải...</div>
          ) : filteredUsers.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              Không có người dùng nào
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b">
                    <th className="text-left p-2">ID</th>
                    <th className="text-left p-2">Họ tên</th>
                    <th className="text-left p-2">Email</th>
                    <th className="text-left p-2">Số điện thoại</th>
                    <th className="text-left p-2">Giới tính</th>
                    <th className="text-left p-2">Vai trò</th>
                    <th className="text-left p-2">Trạng thái</th>
                    <th className="text-left p-2">Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredUsers.map((user: User) => (
                    <tr key={user.id} className="border-b hover:bg-muted/50">
                      <td className="p-2">
                        <span className="text-sm text-muted-foreground font-mono">
                          #{user.id}
                        </span>
                      </td>
                      <td className="p-2">
                        <div>
                          <div className="font-medium">{user.fullName}</div>
                          <div className="text-sm text-muted-foreground">
                            {user.dateOfBirth &&
                              new Date(user.dateOfBirth).toLocaleDateString(
                                "vi-VN"
                              )}
                          </div>
                        </div>
                      </td>
                      <td className="p-2">{user.email}</td>
                      <td className="p-2">{user.phoneNumber}</td>
                      <td className="p-2">{getGenderLabel(user.gender)}</td>
                      <td className="p-2">
                        <Badge variant="outline">
                          {getRoleLabel(user.role)}
                        </Badge>
                      </td>
                      <td className="p-2">
                        <Badge variant={user.status ? "default" : "secondary"}>
                          {user.status ? "Hoạt động" : "Ẩn"}
                        </Badge>
                      </td>
                      <td className="p-2">
                        <div className="flex gap-2">
                          <Button
                            variant="outline"
                            size="sm"
                            onClick={() => openEditModal(user)}
                          >
                            <Edit className="w-4 h-4" />
                          </Button>
                          <AlertDialog
                            open={deletingId === user.id}
                            onOpenChange={(open) =>
                              setDeletingId(open ? user.id : null)
                            }
                          >
                            <AlertDialogTrigger asChild>
                              <Button variant="outline" size="sm">
                                <Trash2 className="w-4 h-4" />
                              </Button>
                            </AlertDialogTrigger>
                            <AlertDialogContent>
                              <AlertDialogHeader>
                                <AlertDialogTitle>
                                  Xác nhận xóa
                                </AlertDialogTitle>
                                <AlertDialogDescription>
                                  Bạn có chắc chắn muốn xóa người dùng "
                                  {user.fullName}"? Hành động này không thể hoàn
                                  tác.
                                </AlertDialogDescription>
                              </AlertDialogHeader>
                              <AlertDialogFooter>
                                <AlertDialogCancel>Hủy</AlertDialogCancel>
                                <AlertDialogAction
                                  onClick={() => handleDeleteUser(user.id)}
                                  disabled={isDeleting}
                                  className="bg-red-600 hover:bg-red-700"
                                >
                                  {isDeleting ? "Đang xóa..." : "Xóa"}
                                </AlertDialogAction>
                              </AlertDialogFooter>
                            </AlertDialogContent>
                          </AlertDialog>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Edit Modal */}
      <Dialog open={isEditModalOpen} onOpenChange={setIsEditModalOpen}>
        <DialogContent className="max-w-2xl max-h-[90vh] overflow-y-auto">
          <DialogHeader>
            <DialogTitle>Sửa người dùng</DialogTitle>
          </DialogHeader>
          {editingUser && (
            <UserUpdateForm
              onSubmit={handleUpdateUser}
              isLoading={isUpdating}
              initialData={editingUser}
            />
          )}
        </DialogContent>
      </Dialog>
    </div>
  );
}

interface UserCreateFormProps {
  onSubmit: (data: UserCreate) => void;
  isLoading: boolean;
}

interface UserUpdateFormProps {
  onSubmit: (data: UserUpdate) => void;
  isLoading: boolean;
  initialData: User;
}

function UserCreateForm({ onSubmit, isLoading }: UserCreateFormProps) {
  const [formData, setFormData] = useState<UserCreate>({
    fullName: "",
    dateOfBirth: "",
    address: "",
    gender: 0,
    phoneNumber: "",
    email: "",
    password: "",
    role: 1,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  const handleInputChange = (field: keyof UserCreate, value: any) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="fullName">Họ tên *</Label>
          <Input
            id="fullName"
            value={formData.fullName}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("fullName", e.target.value)
            }
            required
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="email">Email *</Label>
          <Input
            id="email"
            type="email"
            value={formData.email}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("email", e.target.value)
            }
            required
          />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="dateOfBirth">Ngày sinh *</Label>
          <Input
            id="dateOfBirth"
            type="date"
            value={formData.dateOfBirth}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("dateOfBirth", e.target.value)
            }
            required
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="phoneNumber">Số điện thoại *</Label>
          <Input
            id="phoneNumber"
            value={formData.phoneNumber}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("phoneNumber", e.target.value)
            }
            required
          />
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="address">Địa chỉ *</Label>
        <Input
          id="address"
          value={formData.address}
          onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
            handleInputChange("address", e.target.value)
          }
          required
        />
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="gender">Giới tính *</Label>
          <Select
            value={formData.gender.toString()}
            onValueChange={(value) =>
              handleInputChange("gender", parseInt(value))
            }
          >
            <SelectTrigger>
              <SelectValue placeholder="Chọn giới tính" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="0">Nam</SelectItem>
              <SelectItem value="1">Nữ</SelectItem>
              <SelectItem value="2">Khác</SelectItem>
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-2">
          <Label htmlFor="role">Vai trò *</Label>
          <Select
            value={formData.role.toString()}
            onValueChange={(value) =>
              handleInputChange("role", parseInt(value))
            }
          >
            <SelectTrigger>
              <SelectValue placeholder="Chọn vai trò" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="0">User</SelectItem>
              <SelectItem value="1">Admin</SelectItem>
              <SelectItem value="2">CEO</SelectItem>
              <SelectItem value="3">Manager</SelectItem>
            </SelectContent>
          </Select>
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="password">Mật khẩu *</Label>
        <Input
          id="password"
          type="password"
          value={formData.password}
          onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
            handleInputChange("password", e.target.value)
          }
          required
        />
      </div>

      <div className="flex justify-end space-x-2 pt-4">
        <Button type="submit" disabled={isLoading}>
          {isLoading ? "Đang lưu..." : "Tạo người dùng"}
        </Button>
      </div>
    </form>
  );
}

function UserUpdateForm({
  onSubmit,
  isLoading,
  initialData,
}: UserUpdateFormProps) {
  const [formData, setFormData] = useState<UserUpdate>({
    fullName: initialData.fullName,
    dateOfBirth: initialData.dateOfBirth.slice(0, 10),
    address: initialData.address,
    gender: initialData.gender,
    phoneNumber: initialData.phoneNumber,
    email: initialData.email,
    password: "",
    status: initialData.status,
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit(formData);
  };

  const handleInputChange = (field: keyof UserUpdate, value: any) => {
    setFormData((prev) => ({ ...prev, [field]: value }));
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="fullName">Họ tên *</Label>
          <Input
            id="fullName"
            value={formData.fullName}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("fullName", e.target.value)
            }
            required
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="email">Email *</Label>
          <Input
            id="email"
            type="email"
            value={formData.email}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("email", e.target.value)
            }
            required
          />
        </div>
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="dateOfBirth">Ngày sinh *</Label>
          <Input
            id="dateOfBirth"
            type="date"
            value={formData.dateOfBirth}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("dateOfBirth", e.target.value)
            }
            required
          />
        </div>
        <div className="space-y-2">
          <Label htmlFor="phoneNumber">Số điện thoại *</Label>
          <Input
            id="phoneNumber"
            value={formData.phoneNumber}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("phoneNumber", e.target.value)
            }
            required
          />
        </div>
      </div>

      <div className="space-y-2">
        <Label htmlFor="address">Địa chỉ *</Label>
        <Input
          id="address"
          value={formData.address}
          onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
            handleInputChange("address", e.target.value)
          }
          required
        />
      </div>

      <div className="grid grid-cols-2 gap-4">
        <div className="space-y-2">
          <Label htmlFor="gender">Giới tính *</Label>
          <Select
            value={formData.gender.toString()}
            onValueChange={(value) =>
              handleInputChange("gender", parseInt(value))
            }
          >
            <SelectTrigger>
              <SelectValue placeholder="Chọn giới tính" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="0">Nam</SelectItem>
              <SelectItem value="1">Nữ</SelectItem>
              <SelectItem value="2">Khác</SelectItem>
            </SelectContent>
          </Select>
        </div>
        <div className="space-y-2">
          <Label htmlFor="password">Mật khẩu (để trống nếu không đổi)</Label>
          <Input
            id="password"
            type="password"
            value={formData.password}
            onChange={(e: React.ChangeEvent<HTMLInputElement>) =>
              handleInputChange("password", e.target.value)
            }
          />
        </div>
      </div>

      <div className="flex items-center space-x-2">
        <Switch
          id="status"
          checked={formData.status}
          onCheckedChange={(checked: boolean) =>
            handleInputChange("status", checked)
          }
        />
        <Label htmlFor="status">Trạng thái hoạt động</Label>
      </div>

      <div className="flex justify-end space-x-2 pt-4">
        <Button type="submit" disabled={isLoading}>
          {isLoading ? "Đang lưu..." : "Cập nhật"}
        </Button>
      </div>
    </form>
  );
}
