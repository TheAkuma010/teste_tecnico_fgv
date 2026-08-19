"use client";

import Link from "next/link";
import { FormEvent, useEffect, useState } from "react";
import { Modal } from "@/components/Modal";
import { PageHeader } from "@/components/PageHeader";
import { StatusMessage } from "@/components/StatusMessage";
import { apiClient, getErrorMessage } from "@/lib/apiClient";
import { formatCnpj, formatCurrency, formatDate } from "@/lib/formatters";
import type { Client, Order } from "@/lib/types";

export default function HomePage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [clients, setClients] = useState<Client[]>([]);
  const [clientId, setClientId] = useState("");
  const [dateFrom, setDateFrom] = useState("");
  const [dateTo, setDateTo] = useState("");
  const [clientCnpj, setClientCnpj] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  async function loadOrders() {
    setLoading(true);
    setError(null);

    try {
      const data = await apiClient.orders.list({
        clientId: clientId ? Number(clientId) : undefined,
        dateFrom,
        dateTo
      });
      setOrders(data);
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    apiClient.clients
      .list()
      .then(setClients)
      .catch((loadError) => setError(getErrorMessage(loadError)));
    loadOrders();
  }, []);

  async function handleFilter(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    await loadOrders();
  }

  async function handleCreateOrder(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage(null);
    setError(null);

    try {
      const order = await apiClient.orders.create({ clientCnpj });
      setMessage("Pedido criado com sucesso.");
      setClientCnpj("");
      setIsModalOpen(false);
      await loadOrders();
      window.location.href = `/pedidos/${order.id}`;
    } catch (createError) {
      setError(getErrorMessage(createError));
    }
  }

  return (
    <>
      <PageHeader
        title="Pedidos"
        action={
          <button
            type="button"
            className="btn-primary"
            onClick={() => setIsModalOpen(true)}
          >
            Novo pedido
          </button>
        }
      />

      <div className="mb-4 space-y-3">
        <StatusMessage type="success" message={message} />
        <StatusMessage type="error" message={error} />
      </div>

      <form
        onSubmit={handleFilter}
        className="mb-5 grid gap-3 rounded-lg border border-line bg-white p-4 md:grid-cols-4"
      >
        <div>
          <label className="label" htmlFor="clientId">
            Cliente
          </label>
          <select
            id="clientId"
            className="field"
            value={clientId}
            onChange={(event) => setClientId(event.target.value)}
          >
            <option value="">Todos</option>
            {clients.map((client) => (
              <option key={client.id} value={client.id}>
                {client.name}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label className="label" htmlFor="dateFrom">
            Data inicial
          </label>
          <input
            id="dateFrom"
            type="date"
            className="field"
            value={dateFrom}
            onChange={(event) => setDateFrom(event.target.value)}
          />
        </div>
        <div>
          <label className="label" htmlFor="dateTo">
            Data final
          </label>
          <input
            id="dateTo"
            type="date"
            className="field"
            value={dateTo}
            onChange={(event) => setDateTo(event.target.value)}
          />
        </div>
        <div className="flex items-end">
          <button type="submit" className="btn-secondary w-full">
            Filtrar
          </button>
        </div>
      </form>

      <section className="overflow-hidden rounded-lg border border-line bg-white">
        <table className="w-full text-left text-sm">
          <thead className="bg-slate-100 text-slate-700">
            <tr>
              <th className="px-4 py-3">Pedido</th>
              <th className="px-4 py-3">Cliente</th>
              <th className="px-4 py-3">Data</th>
              <th className="px-4 py-3">Total</th>
              <th className="px-4 py-3">Acoes</th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td className="px-4 py-5" colSpan={5}>
                  Carregando pedidos...
                </td>
              </tr>
            ) : orders.length === 0 ? (
              <tr>
                <td className="px-4 py-5" colSpan={5}>
                  Nenhum pedido encontrado.
                </td>
              </tr>
            ) : (
              orders.map((order) => (
                <tr key={order.id} className="border-t border-line">
                  <td className="px-4 py-3">#{order.id}</td>
                  <td className="px-4 py-3">{order.clientName}</td>
                  <td className="px-4 py-3">{formatDate(order.createdAt)}</td>
                  <td className="px-4 py-3 font-semibold">
                    {formatCurrency(order.total)}
                  </td>
                  <td className="space-x-2 px-4 py-3">
                    <Link className="text-brand underline" href={`/pedidos/${order.id}`}>
                      Editar
                    </Link>
                    <Link
                      className="text-brand underline"
                      href={`/pedidos/${order.id}/detalhes`}
                    >
                      Detalhes
                    </Link>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </section>

      <Modal
        title="Criar pedido"
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
      >
        <form onSubmit={handleCreateOrder} className="space-y-4">
          <div>
            <label className="label" htmlFor="clientCnpj">
              CNPJ do cliente
            </label>
            <input
              id="clientCnpj"
              className="field"
              value={formatCnpj(clientCnpj)}
              onChange={(event) => setClientCnpj(event.target.value)}
              placeholder="00.000.000/0000-00"
            />
          </div>
          <div className="flex justify-end gap-2">
            <button
              type="button"
              className="btn-secondary"
              onClick={() => setIsModalOpen(false)}
            >
              Cancelar
            </button>
            <button type="submit" className="btn-primary">
              Criar
            </button>
          </div>
        </form>
      </Modal>
    </>
  );
}
