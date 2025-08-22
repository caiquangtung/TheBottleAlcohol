import { notFound } from "next/navigation";
import OrderDetailClient from "./OrderDetailClient";

export default async function Page({
  params,
}: {
  params: Promise<{ id: string }> | { id: string };
}) {
  const resolved = await params;
  const id = Number(resolved.id);
  if (Number.isNaN(id)) return notFound();
  return <OrderDetailClient id={id} />;
}
