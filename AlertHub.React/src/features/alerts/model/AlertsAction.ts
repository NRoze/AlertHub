import type { ActiveAlertLocation } from "../../shared/model/ActiveAlertLocation";

export type AlertsAction =
  | { type: "ADD_ALERTS"; payload: ActiveAlertLocation[] }
  | { type: "CLEAN_EXPIRED"; now: number };