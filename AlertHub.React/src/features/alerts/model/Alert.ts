import type { AlertType } from "../../shared/model/AlertType";
import type { Location } from "../../shared/model/Location";

export type Alert = {
  id: string;
  type: AlertType;
  title: string;
  description: string;
  locations: Location[];
  receivedAt: Date;
  expiresAt: Date;
};