import { AlertType } from "../../model/AlertType";

export const alertTypeIconMap: Record<AlertType, string> = {
  [AlertType.ROCKET_FIRE]: "🚀",
  [AlertType.UAV_INTRUSION]: "🛸",
  [AlertType.INFILTRATION]: "⚠️",
  [AlertType.PRE_ALERT]: "⚠️",
  [AlertType.EVENT_ENDED]: "✅",
  [AlertType.UNKNOWN]: "❓",
};


export const alertTypeColorMap: Record<AlertType, string> = {
  [AlertType.ROCKET_FIRE]: "#e7060650",
  [AlertType.UAV_INTRUSION]: "#a3440056",
  [AlertType.INFILTRATION]: "#83084c5d",
  [AlertType.PRE_ALERT]: "#cca91040",
  [AlertType.EVENT_ENDED]: "#21ce2942",
  [AlertType.UNKNOWN]: "#6b728050",
};
