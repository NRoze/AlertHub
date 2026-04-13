import { useMemo } from "react";
import { useSettings } from "../context/SettingsContext";
import type { ActiveAlertLocation } from "../../shared/model/ActiveAlertLocation";
import { AlertType } from "../../shared/model/AlertType";
import { getTitleByType } from "../../alerts/services/constants/alertTitleMap";

export const useMonitoredSync = (alerts: ActiveAlertLocation[]) => {
  const { settings, updateSettings } = useSettings();

  const activeLookup = useMemo(() => {
    const map: Record<string, ActiveAlertLocation> = {};
    alerts.forEach(a => {
      map[a.name] = a;
    });
    return map;
  }, [alerts]);

  const mergedLocations = useMemo(() => {
    const now = new Date();

    return settings.monitoredLocations.map(city => {
      const active = activeLookup[city.name];
      
      const isExpired = !active || active.expiresAt <= now;

      if (isExpired) {
        return { 
          ...city, 
          type: AlertType.NO_ALERTS, 
          message: getTitleByType(AlertType.NO_ALERTS), 
          isLive: false 
        };
      }

      return {
        ...city,
        type: active.type,
        message: active.message,
        receivedAt: active.receivedAt,
        isLive: true
      };
    });
  }, [settings.monitoredLocations, activeLookup]); 

  return { mergedLocations, settings, updateSettings };
};