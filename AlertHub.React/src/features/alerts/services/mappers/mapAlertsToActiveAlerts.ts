import { config } from "../../../../config/config";
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
      expiresAt: new Date(alert.receivedAt.getTime() + config.alertsTtlMs),
      message: alert.title,
    }))
  );
}