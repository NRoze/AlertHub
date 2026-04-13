import type { ActiveAlertLocation } from "../../../shared/model/ActiveAlertLocation";
import type { Alert } from "../../model/Alert";

export function mapAlertsToActiveAlerts(alerts: Alert[]): ActiveAlertLocation[] {
  return alerts.flatMap((alert) =>
    alert.locations.map((location) => ({
      id: `${alert.id}-${location.name}`,
      name: location.name,
      location,
      type: alert.type,
      receivedAt: alert.receivedAt,
      expiresAt: alert.expiresAt,
      message: alert.title,
    }))
  );
}