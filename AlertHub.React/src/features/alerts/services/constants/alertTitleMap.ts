import { AlertType } from "../../../shared/model/AlertType";

export const titleToTypeMap = new Map<string, AlertType>([
  ["אין התרעות", AlertType.NO_ALERTS],
  ["חדירת כלי טיס עוין", AlertType.UAV_INTRUSION],
  ["ירי רקטות וטילים", AlertType.ROCKET_FIRE],
  ["חדירת מחבלים", AlertType.INFILTRATION],
  ["האירוע הסתיים", AlertType.EVENT_ENDED],
  ["בדקות הקרובות צפויות להתקבל התרעות באזורך", AlertType.PRE_ALERT],
]);

export const getTitleByType = (type: AlertType): string => {
  return [...titleToTypeMap.entries()]
    .find(([_, alertType]) => alertType === type)?.[0] || '';
};

// Usage: