import { AlertType } from "../../shared/model/AlertType";

export type MonitoredLocation  = {
    id: string;
    name: string;
    type: AlertType;
    recievedAt: Date;
    message: string;
}