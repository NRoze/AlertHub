import { useEffect, useMemo, useReducer } from "react";
import * as signalR from "@microsoft/signalr";
import { alertsReducer, initialAlertsState } from "../store/alertsReducer";
import type { AlertLocationDto } from "../model/AlertLocationDto";
import { mapLocationsToActiveAlerts } from "../services/mappers/mapLocationsToActiveAlerts";
import type { ActiveAlertLocation } from "../../shared/model/ActiveAlertLocation";

const CLEAN_INTERVAL_MS = 5_000;

export function useAlertsSignalR(baseUrl: string): ActiveAlertLocation[] {
  const [state, dispatch] = useReducer(alertsReducer, initialAlertsState);

  useEffect(() => {
    let isMounted = true;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(baseUrl)
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // Handler defined outside so we can turn it off
    const handleNewAlert = (raw: any) => {
      if (!isMounted) return;
      try {
        const rawList: AlertLocationDto[] = Array.isArray(raw) ? raw : [raw];
        const alertLocations = mapLocationsToActiveAlerts(rawList);
        
        // TBD: dispatch the whole list at once
        // to avoid N re-renders. If not, keep the loop.
        alertLocations.forEach((item) => {
          dispatch({ type: "ADD_ALERT", payload: item });
        });
      } catch (err) {
        console.error("[useAlertsSignalR] Parsing error:", err);
      }
    };

    connection.on("newAlert", handleNewAlert);

    const cleanupInterval = setInterval(() => {
      dispatch({ type: "CLEAN_EXPIRED", now: Date.now() });
    }, CLEAN_INTERVAL_MS);

    async function start() {
      try {
        await connection.start();
        if (isMounted) console.log("[useAlertsSignalR] Connected");
      } catch (err) {
        if (isMounted) console.error("[useAlertsSignalR] Start fail:", err);
      }
    }

    start();

    return () => {
      isMounted = false; // Prevent further state updates
      clearInterval(cleanupInterval);
      connection.off("newAlert", handleNewAlert); // Unregister listener
      connection.stop().catch(console.error);    // Shut down fully
    };
  }, [baseUrl]);

  return useMemo(() => Array.from(state.alerts.values()), [state.alerts]);
}


