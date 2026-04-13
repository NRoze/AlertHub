import type { ActiveAlertLocation } from "../../shared/model/ActiveAlertLocation";
import type { Alert } from "../model/Alert";
import type { AlertsAction } from "../model/AlertsAction";
import { mapAlertsToActiveAlerts } from "../services/mappers/mapAlertsToActiveAlerts";

export type AlertsState = {
  alerts: Map<string, Alert>;
  activeLocations: Map<string, ActiveAlertLocation>;
};

export const initialAlertsState: AlertsState = {
  alerts: new Map(),
  activeLocations: new Map(),
};

function addOrUpdateActiveAlerts(
  state: Map<string, ActiveAlertLocation>,
  activeAlerts: ActiveAlertLocation[]
): Map<string, ActiveAlertLocation> {
  if (!activeAlerts || activeAlerts.length === 0) return state;

  let newMap: Map<string, ActiveAlertLocation> | null = null;

  for (let i = 0; i < activeAlerts.length; i++) {
    const alert = activeAlerts[i];
    if (!alert.id) continue;

    const source = newMap ?? state;
    const existing = source.get(alert.name);

    const isNew = !existing;
    const hasTypeChanged = existing ? existing.type !== alert.type : false;

    if (isNew || hasTypeChanged) {
      if (!newMap) {
        newMap = new Map(state);
      }

      if (existing) {
        const recievedAt =
          existing.type === alert.type
            ? existing.recievedAt
            : alert.recievedAt;

        newMap.set(alert.name, {
          ...existing,
          recievedAt,
          expiresAt: alert.expiresAt,
          type: alert.type,
          message: alert.message,
          location: alert.location, // ✅ ensure location stays updated
        });
      } else {
        newMap.set(alert.name, alert);
      }
    }
  }

  return newMap ?? state;
}

function addOrUpdateAlerts(
  state: AlertsState,
  alerts: Alert[]
): AlertsState {
  if (!alerts || alerts.length === 0) return state;

  let newMap: Map<string, Alert> | null = null;

  for (let i = 0; i < alerts.length; i++) {
    const alert = alerts[i];
    if (!alert.id) continue;

    const source = newMap ?? state.alerts;
    const existing = source.get(alert.id);

    if (!existing) {
      if (!newMap) {
        newMap = new Map(state.alerts);
      }

      newMap.set(alert.id, alert);
    }
  }

  const activeAlerts = mapAlertsToActiveAlerts(alerts);
  const activeAlertsMap = addOrUpdateActiveAlerts(
    state.activeLocations,
    activeAlerts
  );

  if (newMap != state.alerts ||
    activeAlertsMap !== state.activeLocations) {
    return {
      alerts: newMap ?? state.alerts,
      activeLocations: activeAlertsMap,
    };
  }

  return state;
}

export function alertsReducer(
  state: AlertsState,
  action: AlertsAction
): AlertsState {
  switch (action.type) {
    case "ADD_ALERTS":
      return addOrUpdateAlerts(state, action.payload);

    case "CLEAN_EXPIRED": {
      const alerts = new Map(
        Array.from(state.alerts.entries()).filter(
          ([, alert]) => alert.expiresAt.getTime() > action.now
        )
      );

      const activeLocations = new Map(
        Array.from(state.activeLocations.entries()).filter(
          ([, alert]) => alert.expiresAt.getTime() > action.now
        )
      );

      return { alerts, activeLocations };
    }

    default:
      return state;
  }
}