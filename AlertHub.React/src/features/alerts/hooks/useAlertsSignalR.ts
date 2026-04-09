import { useEffect, useMemo, useReducer, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { alertsReducer, initialAlertsState } from "../store/alertsReducer";
import type { AlertLocationDto } from "../model/AlertLocationDto";
import { mapLocationsToActiveAlerts } from "../services/mappers/mapLocationsToActiveAlerts";

const CLEAN_INTERVAL_MS = 5_000;
const RETRY_INTERVAL_MS = 5_000;

export type ConnectionStatus = "Connecting" | "Connected" | "Reconnecting" | "Disconnected";

export function useAlertsSignalR(baseUrl: string) {
  const [state, dispatch] = useReducer(alertsReducer, initialAlertsState);
  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>("Connecting");

  useEffect(() => {
    let isMounted = true;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(baseUrl)
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (_) => {
          return RETRY_INTERVAL_MS; 
        },
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    // How long the client waits for a server ping before giving up
    connection.serverTimeoutInMilliseconds = 60000; // Default is 30s

    // How often the client sends a ping to the server
    connection.keepAliveIntervalInMilliseconds = 7500;

    connection.on("ping", () => {
        console.debug("Heartbeat received"); 
    });
    // --- Connection State Listeners ---
    connection.onreconnecting((error) => {
        console.log("[SignalR] Entering Reconnecting state");
      if (isMounted) setConnectionStatus("Reconnecting");
      console.warn("[SignalR] Connection lost. Reconnecting...", error);
    });

    connection.onreconnected((_) => {
        console.log("[SignalR] Back Online!");
        if (isMounted) setConnectionStatus("Connected"); // <--- CRITICAL
    });
    
    connection.onclose((error) => {
      if (isMounted) setConnectionStatus("Disconnected");
      console.error("[SignalR] Connection closed.", error);
    });

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
        
        if (isMounted) {
          setConnectionStatus("Connected");
          console.log("[useAlertsSignalR] Connected");
        }
      } catch (err) {
        if (isMounted) {
          setConnectionStatus("Disconnected");
          console.error("[useAlertsSignalR] Start fail:", err);
          // Manually trigger a retry here.
          setTimeout(start, 5000);
        }
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

  const alerts = useMemo(() => Array.from(state.alerts.values()), [state.alerts]);

  return { alerts, connectionStatus };
}


