export type GenderType = 0 | 1 | 2; // 0: Male, 1: Female, 2: Other
export type RoleType = 0 | 1 | 2 | 3; // 0: User, 1: Admin, 2: CEO, 3: Manager

export interface User {
  id: number;
  fullName: string;
  dateOfBirth: string;
  address: string;
  gender: GenderType;
  phoneNumber: string;
  email: string;
  role: RoleType;
  status: boolean;
  createdAt: string;
  updatedAt?: string;
  oAuthProvider?: string;
  avatarUrl?: string;
}

export interface UserCreate {
  fullName: string;
  dateOfBirth: string;
  address: string;
  gender: GenderType;
  phoneNumber: string;
  email: string;
  password: string;
  role: RoleType;
}

export interface UserUpdate {
  fullName: string;
  dateOfBirth: string;
  address: string;
  gender: GenderType;
  phoneNumber: string;
  email: string;
  password?: string;
  status: boolean;
}
