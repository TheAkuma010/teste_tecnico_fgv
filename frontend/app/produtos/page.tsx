"use client";

import { FormEvent, useEffect, useState } from "react";
import { PageHeader } from "@/components/PageHeader";
import { StatusMessage } from "@/components/StatusMessage";
import { apiClient, getErrorMessage } from "@/lib/apiClient";
import { formatCurrency } from "@/lib/formatters";
import type { Product } from "@/lib/types";

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [name, setName] = useState("");
  const [price, setPrice] = useState("");
  const [stock, setStock] = useState("");
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);

  async function loadProducts() {
    try {
      setProducts(await apiClient.products.list());
    } catch (loadError) {
      setError(getErrorMessage(loadError));
    }
  }

  useEffect(() => {
    loadProducts();
  }, []);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage(null);
    setError(null);

    try {
      await apiClient.products.create({
        name,
        price: Number(price),
        stock: Number(stock)
      });
      setName("");
      setPrice("");
      setStock("");
      setMessage("Produto cadastrado com sucesso.");
      await loadProducts();
    } catch (createError) {
      setError(getErrorMessage(createError));
    }
  }

  return (
    <>
      <PageHeader title="Cadastro de produtos" />

      <div className="mb-4 space-y-3">
        <StatusMessage type="success" message={message} />
        <StatusMessage type="error" message={error} />
      </div>

      <form
        onSubmit={handleSubmit}
        className="mb-5 grid gap-3 rounded-lg border border-line bg-white p-4 md:grid-cols-[1fr_160px_140px_140px]"
      >
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
          <label className="label" htmlFor="price">
            Preco
          </label>
          <input
            id="price"
            type="number"
            min={0}
            step="0.01"
            className="field"
            value={price}
            onChange={(event) => setPrice(event.target.value)}
            required
          />
        </div>
        <div>
          <label className="label" htmlFor="stock">
            Estoque
          </label>
          <input
            id="stock"
            type="number"
            min={0}
            className="field"
            value={stock}
            onChange={(event) => setStock(event.target.value)}
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
              <th className="px-4 py-3">Produto</th>
              <th className="px-4 py-3">Preco</th>
              <th className="px-4 py-3">Estoque</th>
            </tr>
          </thead>
          <tbody>
            {products.map((product) => (
              <tr key={product.id} className="border-t border-line">
                <td className="px-4 py-3">{product.name}</td>
                <td className="px-4 py-3">{formatCurrency(product.price)}</td>
                <td className="px-4 py-3">{product.stock}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>
    </>
  );
}
