import type { AlertType } from "./AlertType";
import type { Location } from "./Location";

export type ActiveAlertLocation = {
    id: string;
    name: string;
    location: Location;
    type: AlertType;
    recievedAt: Date;
    expiresAt: Date;
    message: string;
}