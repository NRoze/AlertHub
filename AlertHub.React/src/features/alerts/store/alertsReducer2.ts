import type { ActiveAlertLocation } from "../../shared/model/ActiveAlertLocation";
import type { AlertsAction } from "../model/AlertsAction";

export type AlertsState = {
  alerts: Map<string, ActiveAlertLocation>;
};

export const initialAlertsState: AlertsState = { alerts: new Map() };

function addOrUpdateAlerts(state: AlertsState, alerts: ActiveAlertLocation[]): AlertsState {
  if (!alerts || alerts.length === 0) return state;

  let newMap: Map<string, ActiveAlertLocation> | null = null;

  for (let i = 0; i < alerts.length; i++) {
    const alert = alerts[i];
    if (!alert.id) continue;

    const existing = (newMap || state.alerts).get(alert.id);
    const isNew = !existing;
    const hasChanged = existing && (
      existing.expiresAt !== alert.expiresAt ||
      existing.type !== alert.type ||
      existing.message !== alert.message
    );

    if (isNew || hasChanged) {
      if (!newMap) {
        newMap = new Map(state.alerts);
      }

      if (existing) {
        const recievedAt = existing.type === alert.type ? existing.recievedAt : alert.recievedAt;

        newMap.set(alert.id, {
          ...existing,
          recievedAt,
          expiresAt: alert.expiresAt,
          type: alert.type,
          message: alert.message
        });
      } else {
        newMap.set(alert.id, alert);
      }
    }
  }

  return newMap ? { ...state, alerts: newMap } : state;
}


export function alertsReducer(state: AlertsState, action: AlertsAction): AlertsState {
  switch (action.type) {
    case "ADD_ALERTS":
      return addOrUpdateAlerts(state, action.payload);

    case "CLEAN_EXPIRED": {
      const newMap = new Map(
        Array.from(state.alerts.entries()).filter(
          ([, alert]) => alert.expiresAt.getTime() > action.now
        )
      );
      return { alerts: newMap };
    }

    default:
      return state;
  }
}
