import type { Alert } from "../model/Alert";
import type { ActiveAlertLocation } from "../model/ActiveAlertLocation";
import type { Location } from "../model/Location";
import type { AlertsAction } from "../model/AlertsAction";

export type AlertsState = {
  alerts: Map<string, ActiveAlertLocation>;
};

export const initialAlertsState: AlertsState = { alerts: new Map() };

function addOrUpdateAlert(state: AlertsState, alert: ActiveAlertLocation): AlertsState {
  if (alert.id == null) return state;

  const newMap = new Map(state.alerts);
  const existing = newMap.get(alert.id);

  if (existing) {
    newMap.set(alert.id, { ...existing, 
      expiresAt: alert.expiresAt, 
      type: alert.type,
      message: alert.message});
  } else {
    newMap.set(alert.id, alert);
  }

  return { alerts: newMap };
}

function handlePayload(state: AlertsState, alert: ActiveAlertLocation): AlertsState {
  switch (alert.type) {
    case "EVENT_ENDED":
    case "ROCKET_FIRE":
    case "PRE_ALERT":
      return addOrUpdateAlert(state, alert);
    default:
      return state;
  }
}

export function alertsReducer(state: AlertsState, action: AlertsAction): AlertsState {
  switch (action.type) {
    case "ADD_ALERT":
      return handlePayload(state, action.payload);

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
