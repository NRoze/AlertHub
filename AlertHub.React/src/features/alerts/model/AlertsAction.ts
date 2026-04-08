import type { ActiveAlertLocation } from "./ActiveAlertLocation";

export type AlertsAction =
  | { type: "ADD_ALERT"; payload: ActiveAlertLocation }
  | { type: "CLEAN_EXPIRED"; now: number };