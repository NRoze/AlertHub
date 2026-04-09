import { config } from "../../../../config/config";
import type { ActiveAlertLocation } from "../../../shared/model/ActiveAlertLocation";
import type { AlertLocationDto } from "../../model/AlertLocationDto";
import { mapLocation } from "./mapLocations";
import { mapTitleToAlertType } from "./mapTitleToAlertType";

export function mapLocationsToActiveAlerts(dtos: AlertLocationDto[]): ActiveAlertLocation[] {
    return dtos.map(dto => {
        const receivedAtDate = new Date(dto.Timestamp);

        return {
            id: dto.Id,
            location: mapLocation(dto.Id),
            type: mapTitleToAlertType(dto.Title),
            recievedAt: receivedAtDate, 
            expiresAt: new Date(receivedAtDate.getTime() + config.alertsTtlMs),
            message: dto.Title
        } as ActiveAlertLocation;
    });
}