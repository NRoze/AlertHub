import { AlertType } from "../../model/AlertType";

export const titleToTypeMap = new Map<string, AlertType>([
  ["חדירת כלי טיס עוין", AlertType.UAV_INTRUSION],
  ["ירי רקטות וטילים", AlertType.ROCKET_FIRE],
  ["חדירת מחבלים", AlertType.INFILTRATION],
  ["האירוע הסתיים", AlertType.EVENT_ENDED],
  ["בדקות הקרובות צפויות להתקבל התרעות באזורך", AlertType.PRE_ALERT],
]);
