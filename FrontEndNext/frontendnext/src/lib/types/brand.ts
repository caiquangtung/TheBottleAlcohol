export interface Brand {
  id: number;
  name: string;
  description: string;
  logoUrl: string;
  website: string;
  isActive: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface BrandCreateDto {
  name: string;
  description: string;
  logoUrl: string;
  website: string;
  isActive: boolean;
}

export interface BrandUpdateDto {
  name: string;
  description: string;
  logoUrl: string;
  website: string;
  isActive: boolean;
}
