import { AlertType } from "../../../shared/model/AlertType";
import { titleToTypeMap } from "../constants/alertTitleMap";

export function mapTitleToAlertType(title: string): AlertType {
  const type = titleToTypeMap.get(title);

  if (!type) {
    console.warn("Unknown alert title:", title);
    return AlertType.UNKNOWN;
  }

  return type;
}
