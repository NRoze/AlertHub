import { useEffect, useReducer } from "react";
import * as signalR from "@microsoft/signalr";
import { alertsReducer, initialAlertsState } from "../store/alertsReducer";
import type { ActiveAlertLocation } from "../model/ActiveAlertLocation";
import type { AlertLocationDto } from "../model/AlertLocationDto";
import { mapLocationsToActiveAlerts } from "../services/mappers/mapLocationsToActiveAlerts";

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

    const cleanupInterval = setInterval(() => {
      dispatch({ type: "CLEAN_EXPIRED", now: Date.now() });
    }, CLEAN_INTERVAL_MS);

    connection.on("newAlert", (raw: any) => {
        if (!isMounted) return;

        try {
            const rawList: AlertLocationDto[] = Array.isArray(raw) ? raw : [raw];
            const alertLocations: ActiveAlertLocation[] = mapLocationsToActiveAlerts(rawList);
            
            alertLocations.forEach((item) => {
                 dispatch({ type: "ADD_ALERT", payload: item });
        });
        } catch (err) {
            console.error("[useAlertsSignalR] Parsing error:", err);
        }
    });

    const startConnection = async () => {
      try {
        await connection.start();
        
        if (isMounted) {
          console.log("[useAlertsSignalR] Connected to SignalR");
        } else {
          await connection.stop();
        }
      } catch (err: any) {
        if (isMounted && err.name !== "AbortError") {
          console.error("[useAlertsSignalR] Connection failed:", err);
        }
      }
    };

    startConnection();

    return () => {
      isMounted = false; // Prevent further state updates
      clearInterval(cleanupInterval);
      
      // Attempt to stop the connection
      if (connection.state === signalR.HubConnectionState.Connected) {
        connection.stop();
      }
    };
  }, [baseUrl]);

  return Array.from(state.alerts.values());
}


