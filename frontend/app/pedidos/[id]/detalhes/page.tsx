"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { PageHeader } from "@/components/PageHeader";
import { StatusMessage } from "@/components/StatusMessage";
import { apiClient, getErrorMessage } from "@/lib/apiClient";
import { formatCurrency, formatDate } from "@/lib/formatters";
import type { Order } from "@/lib/types";

type OrderDetailsPageProps = {
  params: {
    id: string;
  };
};

export default function OrderDetailsPage({ params }: OrderDetailsPageProps) {
  const orderId = Number(params.id);
  const [order, setOrder] = useState<Order | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    apiClient.orders
      .get(orderId)
      .then(setOrder)
      .catch((loadError) => setError(getErrorMessage(loadError)));
  }, [orderId]);

  return (
    <>
      <PageHeader
        title={`Detalhes do pedido #${orderId}`}
        action={
          <Link className="btn-secondary" href={`/pedidos/${orderId}`}>
            Editar pedido
          </Link>
        }
      />

      <StatusMessage type="error" message={error} />

      <section className="mt-4 rounded-lg border border-line bg-white p-4">
        <dl className="grid gap-4 md:grid-cols-3">
          <div>
            <dt className="text-sm text-slate-600">Cliente</dt>
            <dd className="font-semibold">{order?.clientName ?? "-"}</dd>
          </div>
          <div>
            <dt className="text-sm text-slate-600">Data</dt>
            <dd className="font-semibold">
              {order ? formatDate(order.createdAt) : "-"}
            </dd>
          </div>
          <div>
            <dt className="text-sm text-slate-600">Total</dt>
            <dd className="text-xl font-bold text-brand">
              {formatCurrency(order?.total ?? 0)}
            </dd>
          </div>
        </dl>
      </section>

      <section className="mt-5 overflow-hidden rounded-lg border border-line bg-white">
        <table className="w-full text-left text-sm">
          <thead className="bg-slate-100 text-slate-700">
            <tr>
              <th className="px-4 py-3">Produto</th>
              <th className="px-4 py-3">Quantidade</th>
              <th className="px-4 py-3">Preco unitario</th>
              <th className="px-4 py-3">Total</th>
            </tr>
          </thead>
          <tbody>
            {order?.items.length ? (
              order.items.map((item) => (
                <tr key={item.productId} className="border-t border-line">
                  <td className="px-4 py-3">{item.productName}</td>
                  <td className="px-4 py-3">{item.quantity}</td>
                  <td className="px-4 py-3">
                    {formatCurrency(item.unitPrice)}
                  </td>
                  <td className="px-4 py-3 font-semibold">
                    {formatCurrency(item.total)}
                  </td>
                </tr>
              ))
            ) : (
              <tr>
                <td className="px-4 py-5" colSpan={4}>
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
