import type { AlertType } from "../../shared/model/AlertType.ts";
import type { MonitoredLocation } from "../model/MonitoredLocation.ts";

export function getTimeoutByType(type: AlertType, timeouts: AlertTimeouts): number {
  // Map the enum members to the specific settings keys
  const mapping: Record<string, keyof AlertTimeouts> = {
    "ROCKET_FIRE": "ROCKET_FIRE",
    "UAV_INTRUSION": "UAV_INTRUSION",
    "INFILTRATION": "INFILTRATION",
    "PRE_ALERT": "PRE_ALERT",
    "EVENT_ENDED": "EVENT_ENDED",
  };

  const key = mapping[type] || "UNKNOWN";
  return timeouts[key];
}
export interface AlertTimeouts {
  ROCKET_FIRE: number;
  UAV_INTRUSION: number;
  INFILTRATION: number;
  PRE_ALERT: number;
  EVENT_ENDED: number;
  UNKNOWN: number;
}

export interface UserSettings {
  version: number;
  monitoredLocations: MonitoredLocation[];
  alertTimeouts: AlertTimeouts;
}

export const DEFAULT_SETTINGS: UserSettings = {
  version: 1,
  monitoredLocations: [],
  alertTimeouts: {
    PRE_ALERT: 600000, // 10 minutes
    ROCKET_FIRE: 600000, // 10 minutes
    EVENT_ENDED: 600000, // 10 minutes
    UAV_INTRUSION: 600000, // 10 minutes
    INFILTRATION: 600000, // 10 minutes
    UNKNOWN: 600000 // 10 minutes
  }
};