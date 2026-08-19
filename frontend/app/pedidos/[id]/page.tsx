"use client";

import Link from "next/link";
import { FormEvent, useEffect, useState } from "react";
import { PageHeader } from "@/components/PageHeader";
import { StatusMessage } from "@/components/StatusMessage";
import { apiClient, getErrorMessage } from "@/lib/apiClient";
import { formatCurrency } from "@/lib/formatters";
import type { Order, Product } from "@/lib/types";

type OrderPageProps = {
  params: {
    id: string;
  };
};

export default function OrderPage({ params }: OrderPageProps) {
  const orderId = Number(params.id);
  const [order, setOrder] = useState<Order | null>(null);
  const [products, setProducts] = useState<Product[]>([]);
  const [productId, setProductId] = useState("");
  const [quantity, setQuantity] = useState(1);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function loadData() {
    setError(null);

    try {
      const [orderData, productData] = await Promise.all([
        apiClient.orders.get(orderId),
        apiClient.products.list()
      ]);

      setOrder(orderData);
      setProducts(productData);
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    }
  }

  useEffect(() => {
    loadData();
  }, [orderId]);

  async function handleAddItem(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage(null);
    setError(null);

    try {
      const updatedOrder = await apiClient.orders.addItem(orderId, {
        productId: Number(productId),
        quantity
      });
      setOrder(updatedOrder);
      setProductId("");
      setQuantity(1);
      setMessage("Produto adicionado ao pedido.");
      setProducts(await apiClient.products.list());
    } catch (addError) {
      setError(getErrorMessage(addError));
    }
  }

  async function handleUpdateItem(productIdToUpdate: number, nextQuantity: number) {
    setMessage(null);
    setError(null);

    try {
      const updatedOrder = await apiClient.orders.updateItem(
        orderId,
        productIdToUpdate,
        { quantity: nextQuantity }
      );
      setOrder(updatedOrder);
      setMessage("Item atualizado.");
      setProducts(await apiClient.products.list());
    } catch (updateError) {
      setError(getErrorMessage(updateError));
    }
  }

  async function handleRemoveItem(productIdToRemove: number) {
    const confirmed = window.confirm("Deseja remover este produto do pedido?");

    if (!confirmed) {
      return;
    }

    setMessage(null);
    setError(null);

    try {
      const updatedOrder = await apiClient.orders.removeItem(
        orderId,
        productIdToRemove
      );
      setOrder(updatedOrder);
      setMessage("Item removido.");
      setProducts(await apiClient.products.list());
    } catch (removeError) {
      setError(getErrorMessage(removeError));
    }
  }

  return (
    <>
      <PageHeader
        title={`Pedido #${orderId}`}
        action={
          <Link className="btn-secondary" href={`/pedidos/${orderId}/detalhes`}>
            Ver detalhes
          </Link>
        }
      />

      <div className="mb-4 space-y-3">
        <StatusMessage type="success" message={message} />
        <StatusMessage type="error" message={error} />
      </div>

      <div className="mb-5 rounded-lg border border-line bg-white p-4">
        <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-sm text-slate-600">Cliente</p>
            <p className="font-semibold text-ink">
              {order?.clientName ?? "Carregando..."}
            </p>
          </div>
          <div>
            <p className="text-sm text-slate-600">Total do pedido</p>
            <p className="text-2xl font-bold text-brand">
              {formatCurrency(order?.total ?? 0)}
            </p>
          </div>
        </div>
      </div>

      <form
        onSubmit={handleAddItem}
        className="mb-5 grid gap-3 rounded-lg border border-line bg-white p-4 md:grid-cols-[1fr_140px_140px]"
      >
        <div>
          <label className="label" htmlFor="productId">
            Produto
          </label>
          <select
            id="productId"
            className="field"
            value={productId}
            onChange={(event) => setProductId(event.target.value)}
            required
          >
            <option value="">Selecione</option>
            {products.map((product) => (
              <option key={product.id} value={product.id}>
                {product.name} - estoque {product.stock} -{" "}
                {formatCurrency(product.price)}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label className="label" htmlFor="quantity">
            Quantidade
          </label>
          <input
            id="quantity"
            type="number"
            min={1}
            className="field"
            value={quantity}
            onChange={(event) => setQuantity(Number(event.target.value))}
          />
        </div>
        <div className="flex items-end">
          <button type="submit" className="btn-primary w-full">
            Adicionar
          </button>
        </div>
      </form>

      <section className="overflow-hidden rounded-lg border border-line bg-white">
        <table className="w-full text-left text-sm">
          <thead className="bg-slate-100 text-slate-700">
            <tr>
              <th className="px-4 py-3">Produto</th>
              <th className="px-4 py-3">Quantidade</th>
              <th className="px-4 py-3">Preco unitario</th>
              <th className="px-4 py-3">Total</th>
              <th className="px-4 py-3">Acoes</th>
            </tr>
          </thead>
          <tbody>
            {order?.items.length ? (
              order.items.map((item) => (
                <tr key={item.productId} className="border-t border-line">
                  <td className="px-4 py-3">{item.productName}</td>
                  <td className="px-4 py-3">
                    <input
                      type="number"
                      min={1}
                      className="field max-w-24"
                      value={item.quantity}
                      onChange={(event) =>
                        handleUpdateItem(
                          item.productId,
                          Number(event.target.value)
                        )
                      }
                    />
                  </td>
                  <td className="px-4 py-3">
                    {formatCurrency(item.unitPrice)}
                  </td>
                  <td className="px-4 py-3 font-semibold">
                    {formatCurrency(item.total)}
                  </td>
                  <td className="px-4 py-3">
                    <button
                      type="button"
                      className="btn-danger"
                      onClick={() => handleRemoveItem(item.productId)}
                    >
                      Remover
                    </button>
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td className="px-4 py-5" colSpan={5}>
                  Nenhum produto no pedido.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </section>
    </>
  );
}
