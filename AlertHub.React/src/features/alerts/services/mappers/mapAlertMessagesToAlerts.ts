import { config } from "../../../../config/config";
import type { Alert } from "../../model/Alert";
import type { AlertMessage } from "../../model/AlertMessage";
import { mapLocations } from "./mapLocations";
import { mapTitleToAlertType } from "./mapTitleToAlertType";

export function mapAlertMessagesToAlerts(dtos: AlertMessage[]): Alert[] {
    return dtos.map(dto => {
        const receivedAt = dto.timestamp > 0 ? dto.timestamp : Date.now();
        const expiresAt = dto.expireAt > 0 ? dto.expireAt : receivedAt + config.alertsTtlMs;

        return {
            id: dto.id,
            type: mapTitleToAlertType(dto.title),
            title: dto.title,
            description: dto.desc,
            locations: mapLocations(dto.data),
            receivedAt: new Date(receivedAt), 
            expiresAt: new Date(expiresAt)
        } as Alert;
    });
}