import { useEffect, useReducer } from "react";
import * as signalR from "@microsoft/signalr";
import type { AlertMessage } from "../model/AlertMessage";
import { normalizeAlert } from "../services/normalizeAlert";
import { alertsReducer, initialAlertsState } from "../store/alertsReducer";
import type { ActiveAlertLocation } from "../model/ActiveAlertLocation";

const CLEAN_INTERVAL_MS = 5_000;

function parseData(raw: string): AlertMessage {
  const first = JSON.parse(raw);
  return typeof first === "string" ? JSON.parse(first) : first;
}

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
            const data = parseData(raw);
            const alertsArray = Array.isArray(data) ? data : [data];

            alertsArray.forEach((item) => {
                if (!item || typeof item !== 'object') return;
                dispatch({ type: "ADD_ALERT", payload: normalizeAlert(item) });
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