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
    PRE_ALERT: 20000, // 20 seconds
    ROCKET_FIRE: 20000, // 20 seconds
    EVENT_ENDED: 2000, // 20 seconds
    UAV_INTRUSION: 20000, // 20 seconds
    INFILTRATION: 20000, // 20 seconds
    UNKNOWN: 20000 // 20 seconds
  }
};