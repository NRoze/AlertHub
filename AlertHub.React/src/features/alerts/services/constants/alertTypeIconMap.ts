import { AlertType } from "../../model/AlertType";

export const alertTypeIconMap: Record<AlertType, string> = {
  [AlertType.ROCKET_FIRE]: "🚀",
  [AlertType.UAV_INTRUSION]: "🛸",
  [AlertType.INFILTRATION]: "⚠️",
  [AlertType.PRE_ALERT]: "⚠️",
  [AlertType.EVENT_ENDED]: "✅",
  [AlertType.UNKNOWN]: "❓",
};


export const alertTypeLineColorMap: Record<AlertType, string> = {
  [AlertType.ROCKET_FIRE]: "black",
  [AlertType.UAV_INTRUSION]: "black",
  [AlertType.INFILTRATION]: "black",
  [AlertType.PRE_ALERT]: "black",
  [AlertType.EVENT_ENDED]: "black",
  [AlertType.UNKNOWN]: "black",
};
