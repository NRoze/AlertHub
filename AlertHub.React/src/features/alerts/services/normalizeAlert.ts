import type { Alert } from "../model/Alert";
import type { AlertMessage } from "../model/AlertMessage";
import { normalizeTitle } from "./normalizeTitle";
import { mapTitleToAlertType } from "./mappers/mapTitleToAlertType";
import { mapLocations } from "./mappers/mapLocations";

const ALERT_TTL_MS = 2 * 60 * 1000; // 2 minutes

export function normalizeAlert(raw: AlertMessage): Alert {
  const normalizedTitle = normalizeTitle(raw.title);
  const type = mapTitleToAlertType(normalizedTitle);
  const locations = mapLocations(raw.data ?? []);

  const receivedAt = new Date();
  const expiresAt = new Date(receivedAt.getTime() + ALERT_TTL_MS);

  return {
    id: raw.id,
    type,
    title: raw.title, // keep original for UI
    description: raw.desc,
    locations,
    receivedAt,
    expiresAt,
  };
}