import type {
  PersonDto,
  OrderDto,
  BalanceSummaryDto,
  PersonMap,
  OrderItemDto,
  SettlementDto,
  PairwiseBalanceDto,
} from "../types/dto";

const API = import.meta.env.VITE_API_BASE_URL;
const GROUP_ID = import.meta.env.VITE_GROUP_ID;

const baseUrl = (path: string) => `${API}${path}`;
const groupBase = baseUrl(`/api/groups/${GROUP_ID}`);
const orderBase = baseUrl(`/api/orders/${GROUP_ID}`);
const presenceBase = baseUrl(`/api/presences/${GROUP_ID}`);
const balanceBase = baseUrl(`/api/balances/${GROUP_ID}`);

export async function fetchGroup(): Promise<{ members: PersonDto[] }> {
  const res = await fetch(groupBase);
  if (!res.ok) throw new Error("Failed to fetch group");
  return res.json();
}

export async function fetchGroupMembers(): Promise<PersonDto[]> {
  return (await fetchGroup()).members;
}

export async function fetchPersonMap(): Promise<PersonMap> {
  const { members } = await fetchGroup();
  return Object.fromEntries(members.map((m) => [m.personId, m.name]));
}

export async function fetchPresenceFromServer(
  date?: string
): Promise<Record<string, boolean>> {
  const url = new URL(presenceBase);
  if (date) url.searchParams.set("date", date);
  const res = await fetch(url.toString());
  if (!res.ok) return {};
  const arr: { personId: string; isPresent: boolean }[] = await res.json();
  return Object.fromEntries(arr.map((p) => [p.personId, p.isPresent]));
}

export async function markPresence(
  personId: string,
  isPresent: boolean
): Promise<void> {
  const url = new URL(`${presenceBase}/people/${personId}`);
  url.searchParams.set("isPresent", String(isPresent));
  const res = await fetch(url.toString(), { method: "POST" });
  if (!res.ok) throw new Error("Failed to mark presence");
}

export async function fetchRecommendedPayer(): Promise<PersonDto | null> {
  const res = await fetch(`${balanceBase}/next`);
  if (res.status === 204) return null;
  if (!res.ok) throw new Error("Failed to fetch recommended payer");
  return res.json();
}

export async function fetchOrders(): Promise<OrderDto[]> {
  const res = await fetch(orderBase);
  if (!res.ok) return [];
  return res.json();
}

export async function submitOrder(order: {
  payerPersonId: string;
  date: string;
  items: OrderItemDto[];
}): Promise<string> {
  const res = await fetch(orderBase, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(order),
  });
  if (!res.ok) throw new Error("Failed to submit order");
  return res.json();
}

export async function fetchBalances(): Promise<BalanceSummaryDto[]> {
  const res = await fetch(balanceBase);
  if (!res.ok) return [];
  return res.json();
}

export async function fetchPairwise(
  personA: string,
  personB: string
): Promise<PairwiseBalanceDto> {
  const url = new URL(`${balanceBase}/pairwise`);
  url.searchParams.set("personA", personA);
  url.searchParams.set("personB", personB);
  const res = await fetch(url.toString());
  if (!res.ok) throw new Error("Failed to fetch pairwise balance");
  return res.json();
}

export async function settlePairwise(
  fromPersonId: string,
  toPersonId: string,
  amount: number
): Promise<void> {
  const res = await fetch(`${balanceBase}/settle`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ fromPersonId, toPersonId, amount }),
  });
  if (!res.ok) throw new Error("Failed to settle up");
}

export async function fetchSettlements(): Promise<SettlementDto[]> {
  const res = await fetch(`${balanceBase}/simplified-settlements`);
  if (!res.ok) throw new Error("Failed to fetch settlements");
  return res.json();
}

export async function fetchPairwisePositions(): Promise<PairwiseBalanceDto[]> {
  const res = await fetch(`${balanceBase}/pairwise-positions`);
  if (!res.ok) throw new Error("Failed to fetch pairwise positions");
  return res.json();
}
