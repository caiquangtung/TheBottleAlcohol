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
  Id: number;
  FullName: string;
  Email: string;
  Role: string;
  Address: string;
  PhoneNumber: string;
  DateOfBirth: string;
  Gender: string;
  Status: boolean;
  CreatedAt: string;
}

export interface LoginResponse {
  Success: boolean;
  Message: string | null;
  Data: {
    Id: number;
    FullName: string;
    Email: string;
    Role: string;
    AccessToken: string;
    RefreshToken: string;
  };
}

export interface RegisterResponse {
  Success: boolean;
  Message: string | null;
  Data: {
    Id: number;
    FullName: string;
    Email: string;
  };
}

export interface ProfileResponse {
  Success: boolean;
  Message: string | null;
  Data: User;
}
