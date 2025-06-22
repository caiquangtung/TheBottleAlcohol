export interface LoginCredentials {
  email: string;
  password: string;
}

export interface RegisterCredentials {
  fullName: string;
  dateOfBirth: string;
  address: string;
  gender: string;
  phoneNumber: string;
  email: string;
  password: string;
}

export interface User {
  id: number;
  fullName: string;
  email: string;
  role: string;
  address: string;
  phoneNumber: string;
  dateOfBirth: string;
  gender: string;
  status: boolean;
  createdAt: string;
}

export interface LoginResponse {
  success: boolean;
  message: string | null;
  data: {
    id: number;
    fullName: string;
    email: string;
    role: string;
    accessToken: string;
  };
}

export interface RegisterResponse {
  success: boolean;
  message: string | null;
  data: {
    id: number;
    fullName: string;
    email: string;
  };
}

export interface ProfileResponse {
  success: boolean;
  message: string | null;
  data: User;
}
