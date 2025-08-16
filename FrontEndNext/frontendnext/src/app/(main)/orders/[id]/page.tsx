import { notFound } from "next/navigation";
import OrderDetailClient from "./OrderDetailClient";

export default function Page({ params }: { params: { id: string } }) {
  const id = Number(params.id);
  if (Number.isNaN(id)) return notFound();
  return <OrderDetailClient id={id} />;
}
