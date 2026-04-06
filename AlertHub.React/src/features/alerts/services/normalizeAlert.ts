import type { Alert } from "../model/Alert";
import type { AlertMessage } from "../model/AlertMessage";
import { normalizeTitle } from "./normalizeTitle";
import { mapTitleToAlertType } from "./mappers/mapTitleToAlertType";
import { mapLocations } from "./mappers/mapLocations";
import { config } from "../../../config/config";

export function normalizeAlert(raw: AlertMessage): Alert {
  const normalizedTitle = normalizeTitle(raw.title);
  const type = mapTitleToAlertType(normalizedTitle);
  const locations = mapLocations(raw.data ?? []);
  const receivedAt = new Date();
  const expiresAt = new Date(receivedAt.getTime() + config.alertsTtlMs);

  return {
    id: raw.id,
    type,
    title: raw.title,
    description: raw.desc,
    locations,
    receivedAt,
    expiresAt,
  };
}