"use client";

import { FormEvent, useEffect, useState } from "react";
import { PageHeader } from "@/components/PageHeader";
import { StatusMessage } from "@/components/StatusMessage";
import { apiClient, getErrorMessage } from "@/lib/apiClient";
import { formatCnpj, formatDate } from "@/lib/formatters";
import type { Client } from "@/lib/types";

export default function ClientsPage() {
  const [clients, setClients] = useState<Client[]>([]);
  const [cnpj, setCnpj] = useState("");
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function loadClients() {
    try {
      setClients(await apiClient.clients.list());
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    }
  }

  useEffect(() => {
    loadClients();
  }, []);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage(null);
    setError(null);

    try {
      await apiClient.clients.create({ cnpj, name, email });
      setCnpj("");
      setName("");
      setEmail("");
      setMessage("Cliente cadastrado com sucesso.");
      await loadClients();
    } catch (createError) {
      setError(getErrorMessage(createError));
    }
  }

  return (
    <>
      <PageHeader title="Cadastro de clientes" />

      <div className="mb-4 space-y-3">
        <StatusMessage type="success" message={message} />
        <StatusMessage type="error" message={error} />
      </div>

      <form
        onSubmit={handleSubmit}
        className="mb-5 grid gap-3 rounded-lg border border-line bg-white p-4 md:grid-cols-[180px_1fr_1fr_140px]"
      >
        <div>
          <label className="label" htmlFor="cnpj">
            CNPJ
          </label>
          <input
            id="cnpj"
            className="field"
            value={formatCnpj(cnpj)}
            onChange={(event) => setCnpj(event.target.value)}
            placeholder="00.000.000/0000-00"
            required
          />
        </div>
        <div>
          <label className="label" htmlFor="name">
            Nome
          </label>
          <input
            id="name"
            className="field"
            value={name}
            onChange={(event) => setName(event.target.value)}
            required
          />
        </div>
        <div>
          <label className="label" htmlFor="email">
            E-mail
          </label>
          <input
            id="email"
            type="email"
            className="field"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            required
          />
        </div>
        <div className="flex items-end">
          <button type="submit" className="btn-primary w-full">
            Salvar
          </button>
        </div>
      </form>

      <section className="overflow-hidden rounded-lg border border-line bg-white">
        <table className="w-full text-left text-sm">
          <thead className="bg-slate-100 text-slate-700">
            <tr>
              <th className="px-4 py-3">Cliente</th>
              <th className="px-4 py-3">CNPJ</th>
              <th className="px-4 py-3">E-mail</th>
              <th className="px-4 py-3">Cadastro</th>
            </tr>
          </thead>
          <tbody>
            {clients.map((client) => (
              <tr key={client.id} className="border-t border-line">
                <td className="px-4 py-3">{client.name}</td>
                <td className="px-4 py-3">{formatCnpj(client.cnpj)}</td>
                <td className="px-4 py-3">{client.email}</td>
                <td className="px-4 py-3">{formatDate(client.createdAt)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>
    </>
  );
}
