import { AlertType } from "../../shared/model/AlertType";

export type MonitoredLocation  = {
    id: string;
    type: AlertType;
    recievedAt: Date;
    message: string;
}