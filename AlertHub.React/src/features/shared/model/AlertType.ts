export const AlertType = {
  NO_ALERTS: "NO_ALERTS",
  UAV_INTRUSION: "UAV_INTRUSION",
  ROCKET_FIRE: "ROCKET_FIRE",
  INFILTRATION: "INFILTRATION",
  UNKNOWN: "UNKNOWN",
  EVENT_ENDED: "EVENT_ENDED",
  PRE_ALERT: "PRE_ALERT",
} as const;

export type AlertType = (typeof AlertType)[keyof typeof AlertType];