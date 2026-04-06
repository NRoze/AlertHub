import type { AlertType } from "./AlertType";
import type { Location } from "./Location";

export type Alert = {
  id: string;
  type: AlertType;
  title: string;
  description: string;
  locations: Location[];
  receivedAt: Date;
  expiresAt: Date;
};