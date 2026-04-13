import type { Alert } from "./Alert";

export type AlertsAction =
  | { type: "ADD_ALERTS"; payload: Alert[] }
  | { type: "CLEAN_EXPIRED"; now: number };