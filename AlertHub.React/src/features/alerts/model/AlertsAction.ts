import type { Alert } from "./Alert";

export type AlertsAction =
  | { type: "ADD_ALERT"; payload: Alert }
  | { type: "CLEAN_EXPIRED"; now: number };