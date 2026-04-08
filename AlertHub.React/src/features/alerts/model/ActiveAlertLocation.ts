import { AlertType } from "./AlertType";
import type { Location } from "./Location";

export type ActiveAlertLocation = {
    location: Location;
    type: AlertType;
    recievedAt: Date;
    expiresAt: Date;
    id: string;
    message: string;
}