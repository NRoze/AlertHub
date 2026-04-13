import type { ActiveAlertLocation } from "../../../shared/model/ActiveAlertLocation";
import type { Alert } from "../../model/Alert";

export function mapAlertsToActiveAlerts(alerts: Alert[]): ActiveAlertLocation[] {
  return alerts.flatMap((alert) =>
    alert.locations.map((location) => ({
      id: `${alert.id}-${location.name}`,
      name: location.name,
      location,
      type: alert.type,
      recievedAt: alert.receivedAt, // note spelling mismatch in your type
      expiresAt: alert.expiresAt,
      message: alert.title,
    }))
  );
}