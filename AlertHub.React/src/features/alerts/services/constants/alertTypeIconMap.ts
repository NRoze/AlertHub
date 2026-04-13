import { AlertType } from "../../../shared/model/AlertType";

export const alertTypeIconMap: Record<AlertType, string> = {
  [AlertType.NO_ALERTS]: "✅",
  [AlertType.ROCKET_FIRE]: "🚀",
  [AlertType.UAV_INTRUSION]: "🛸",
  [AlertType.INFILTRATION]: "⚠️",
  [AlertType.PRE_ALERT]: "⚠️",
  [AlertType.EVENT_ENDED]: "✅",
  [AlertType.UNKNOWN]: "❓",
};
export const alertTypeMessageMap: Record<AlertType, string> = {
  [AlertType.NO_ALERTS]: "אין התרעות",
  [AlertType.ROCKET_FIRE]: "ירי טילים",
  [AlertType.UAV_INTRUSION]: "חדירת כלי טיס עוין",
  [AlertType.INFILTRATION]: "חדירת מחבלים",
  [AlertType.PRE_ALERT]: "התראה מקדימה",
  [AlertType.EVENT_ENDED]: "האירוע הסתיים",
  [AlertType.UNKNOWN]: "אין מידע",
};


export const alertTypeLineColorMap: Record<AlertType, string> = {
  [AlertType.NO_ALERTS]: "black",
  [AlertType.ROCKET_FIRE]: "black",
  [AlertType.UAV_INTRUSION]: "black",
  [AlertType.INFILTRATION]: "black",
  [AlertType.PRE_ALERT]: "black",
  [AlertType.EVENT_ENDED]: "black",
  [AlertType.UNKNOWN]: "black",
};
