import type { Client, Order, Product } from "./types";

const apiBaseUrl =
  process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5000/api";

type QueryValue = string | number | undefined | null;

class HttpError extends Error {
  statusCode: number;

  constructor(statusCode: number, message: string) {
    super(message);
    this.statusCode = statusCode;
  }
}

async function request<T>(
  path: string,
  options: RequestInit = {}
): Promise<T> {
  const response = await fetch(`${apiBaseUrl}${path}`, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...options.headers
    },
    cache: "no-store"
  });

  if (!response.ok) {
    const error = await response
      .json()
      .catch(() => ({ message: "Nao foi possivel concluir a operacao." }));

    throw new HttpError(
      response.status,
      error.message ?? "Nao foi possivel concluir a operacao."
    );
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return response.json() as Promise<T>;
}

function buildQuery(params: Record<string, QueryValue>) {
  const query = new URLSearchParams();

  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      query.append(key, String(value));
    }
  });

  const queryString = query.toString();

  return queryString ? `?${queryString}` : "";
}

export const apiClient = {
  clients: {
    list: () => request<Client[]>("/clients"),
    create: (payload: { cnpj: string; name: string; email: string }) =>
      request<Client>("/clients", {
        method: "POST",
        body: JSON.stringify(payload)
      })
  },
  products: {
    list: () => request<Product[]>("/products"),
    create: (payload: { name: string; price: number; stock: number }) =>
      request<Product>("/products", {
        method: "POST",
        body: JSON.stringify(payload)
      }),
    update: (
      id: number,
      payload: { name: string; price: number; stock: number }
    ) =>
      request<Product>(`/products/${id}`, {
        method: "PUT",
        body: JSON.stringify(payload)
      })
  },
  orders: {
    list: (params: { clientId?: number; dateFrom?: string; dateTo?: string }) =>
      request<Order[]>(`/orders${buildQuery(params)}`),
    get: (id: number) => request<Order>(`/orders/${id}`),
    create: (payload: { clientCnpj: string }) =>
      request<Order>("/orders", {
        method: "POST",
        body: JSON.stringify(payload)
      }),
    addItem: (orderId: number, payload: { productId: number; quantity: number }) =>
      request<Order>(`/orders/${orderId}/items`, {
        method: "POST",
        body: JSON.stringify(payload)
      }),
    updateItem: (
      orderId: number,
      productId: number,
      payload: { quantity: number }
    ) =>
      request<Order>(`/orders/${orderId}/items/${productId}`, {
        method: "PUT",
        body: JSON.stringify(payload)
      }),
    removeItem: (orderId: number, productId: number) =>
      request<Order>(`/orders/${orderId}/items/${productId}`, {
        method: "DELETE"
      })
  }
};

export function getErrorMessage(error: unknown) {
  if (error instanceof Error) {
    return error.message;
  }

  return "Nao foi possivel concluir a operacao.";
}
