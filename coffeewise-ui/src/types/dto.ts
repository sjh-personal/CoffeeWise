export interface PersonDto {
  personId: string;
  name: string;
  email: string;
}

export interface PresenceDto {
  personId: string;
  date: string;
  isPresent: boolean;
}

export interface OrderItemDto {
  consumerPersonId: string;
  description: string;
  price: number;
}

export interface BalanceSummaryDto {
  personId: string;
  name: string;
  email: string;
  paid: number;
  owes: number;
  balance: number;
}

export type OrderItem = {
  consumerPersonId: string;
  description: string;
  price: number;
};

export type OrderDto = {
  orderId: string;
  payerPersonId: string;
  date: string;
  items: OrderItem[];
};

export type SettlementDto = {
  fromPersonId: string;
  fromName: string;
  toPersonId: string;
  toName: string;
  amount: number;
};

export type PersonMap = Record<string, string>;
