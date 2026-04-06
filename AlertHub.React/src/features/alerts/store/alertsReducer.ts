import type { Alert } from "../model/Alert";
import type { ActiveAlertLocation } from "../model/ActiveAlertLocation";
import type { Location } from "../model/Location";

export type AlertsState = {
  alerts: Map<string, ActiveAlertLocation>;
};

export type AlertsAction =
  | { type: "ADD_ALERT"; payload: Alert }
  | { type: "CLEAN_EXPIRED"; now: number };

export const initialAlertsState: AlertsState = { alerts: new Map() };

function getLocationKey(location: Location): string {
  return `${location.area ?? ""}:${location.name}`;
}

function addOrUpdateAlert(state: AlertsState, alert: Alert): AlertsState {
  if (alert.id == null) return state;

  const newMap = new Map(state.alerts);

  alert.locations?.forEach((location) => {
    const key = getLocationKey(location);
    const existing = newMap.get(key);

    if (existing) {
      newMap.set(key, { ...existing, 
        expiresAt: alert.expiresAt, 
        alertType: alert.type,
        message: alert.title});
    } else {
      newMap.set(key, {
        id: location.name,
        location,
        alertType: alert.type,
        recievedAt: alert.receivedAt,
        expiresAt: alert.expiresAt,
        message: alert.title
      });
    }
  });

  return { alerts: newMap };
}

// function removeAlertLocations(state: AlertsState, alert: Alert): AlertsState {
//   const newMap = new Map(state.alerts);
  
//   alert.locations?.forEach((location) => {
//     newMap.delete(getLocationKey(location));
//   });

//   return { alerts: newMap };
// }

function handlePayload(state: AlertsState, alert: Alert): AlertsState {
  switch (alert.type) {
    case "EVENT_ENDED":
      // return removeAlertLocations(state, alert);
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
