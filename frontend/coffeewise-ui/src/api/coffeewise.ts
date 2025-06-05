import type {
  PersonDto,
  OrderDto,
  BalanceSummaryDto,
  PersonMap,
  OrderItemDto,
} from "../types/dto";
import { GROUP_ID } from "../App";

export async function fetchGroup(): Promise<{ members: PersonDto[] }> {
  const res = await fetch(
    `${import.meta.env.VITE_API_BASE_URL}/api/groups/${GROUP_ID}`
  );
  if (!res.ok) throw new Error("Failed to fetch group");
  return res.json();
}

export async function fetchGroupMembers(): Promise<PersonDto[]> {
  return (await fetchGroup()).members;
}

export async function fetchPersonMap(): Promise<PersonMap> {
  const group = await fetchGroup();
  const map: PersonMap = {};
  for (const m of group.members) map[m.personId] = m.name;
  return map;
}

export async function fetchPresenceFromServer(): Promise<
  Record<string, boolean>
> {
  const res = await fetch(
    `${import.meta.env.VITE_API_BASE_URL}/api/groups/${GROUP_ID}/presences`
  );
  if (!res.ok) return {};
  const arr = await res.json();
  const map: Record<string, boolean> = {};
  for (const p of arr) map[p.personId] = p.isPresent;
  return map;
}

export async function fetchRecommendedPayer(): Promise<PersonDto | null> {
  const res = await fetch(
    `${import.meta.env.VITE_API_BASE_URL}/api/groups/${GROUP_ID}/balances/next`
  );
  if (res.status === 204) return null;
  if (!res.ok) return null;
  return res.json();
}

export async function fetchOrders(): Promise<OrderDto[]> {
  const res = await fetch(
    `${import.meta.env.VITE_API_BASE_URL}/api/groups/${GROUP_ID}/orders`
  );
  if (!res.ok) return [];
  return res.json();
}

export async function fetchBalances(): Promise<BalanceSummaryDto[]> {
  const res = await fetch(
    `${import.meta.env.VITE_API_BASE_URL}/api/groups/${GROUP_ID}/balances`
  );
  if (!res.ok) return [];
  return res.json();
}

export async function submitOrder(order: {
  payerPersonId: string;
  date: string;
  items: OrderItemDto[];
}) {
  const res = await fetch(
    `${import.meta.env.VITE_API_BASE_URL}/api/groups/${GROUP_ID}/orders`,
    {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(order),
    }
  );
  if (!res.ok) throw new Error("Failed to submit order");
  return res.json();
}
