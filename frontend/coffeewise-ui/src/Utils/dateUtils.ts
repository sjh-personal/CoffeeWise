import { format } from "date-fns";

export function formatDate(date: Date = new Date()): string {
  return format(date, "yyyy-MM-dd");
}
