export interface Supplier {
  id: number;
  name: string;
  email: string;
  phoneNumber: string;
  address: string;
  status: string;
  createdDate: string;
}

export interface SupplierCreateDto {
  name: string;
  email: string;
  phoneNumber: string;
}

export interface SupplierUpdateDto {
  name: string;
  email: string;
  phoneNumber: string;
}
