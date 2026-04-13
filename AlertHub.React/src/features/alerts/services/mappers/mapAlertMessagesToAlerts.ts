import { config } from "../../../../config/config";
import type { Alert } from "../../model/Alert";
import type { AlertMessage } from "../../model/AlertMessage";
import { mapLocations } from "./mapLocations";
import { mapTitleToAlertType } from "./mapTitleToAlertType";

export function mapAlertMessagesToAlerts(dtos: AlertMessage[]): Alert[] {
    return dtos.map(dto => {
        const now = Date.now();
        const receivedAtDate = dto.timestamp > 0 ? new Date(dto.timestamp) : new Date(now);

        return {
            id: dto.id,
            type: mapTitleToAlertType(dto.title),
            title: dto.title,
            description: dto.desc,
            locations: mapLocations(dto.data),
            receivedAt: receivedAtDate, 
            expiresAt: new Date(now + config.alertsTtlMs)
        } as Alert;
    });
}