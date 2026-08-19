export type Client = {
  id: number;
  cnpj: string;
  name: string;
  email: string;
  createdAt: string;
};

export type Product = {
  id: number;
  name: string;
  price: number;
  stock: number;
};

export type OrderItem = {
  productId: number;
  productName: string;
  quantity: number;
  unitPrice: number;
  total: number;
};

export type Order = {
  id: number;
  clientId: number;
  clientName: string;
  createdAt: string;
  total: number;
  items: OrderItem[];
};

export type ApiError = {
  statusCode: number;
  message: string;
};
