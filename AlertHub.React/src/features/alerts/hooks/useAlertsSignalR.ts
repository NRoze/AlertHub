import { useEffect, useMemo, useReducer, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { alertsReducer, initialAlertsState } from "../store/alertsReducer";
import type { AlertMessage } from "../model/AlertMessage";
import { mapAlertMessagesToAlerts } from "../services/mappers/mapAlertMessagesToAlerts";
import type { Alert } from "../model/Alert";

const CONNECTION_TIMEOUT_MS = 60_000;
const KEEPALIVE_INTERVAL_MS = 7_500;
const CLEAN_INTERVAL_MS = 60_000;
const RETRY_INTERVAL_MS = 5_000;
const ALERTS_ENDPOINT = "alerts";

export type ConnectionStatus = "Connecting" | "Connected" | "Reconnecting" | "Disconnected";

export function useAlertsSignalR(baseUrl: string) {
  const [state, dispatch] = useReducer(alertsReducer, initialAlertsState);
  const [connectionStatus, setConnectionStatus] = useState<ConnectionStatus>("Connecting");

  useEffect(() => {
    let isMounted = true;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(baseUrl)
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: () => {
          return RETRY_INTERVAL_MS; 
        },
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    connection.serverTimeoutInMilliseconds = CONNECTION_TIMEOUT_MS; 
    connection.keepAliveIntervalInMilliseconds = KEEPALIVE_INTERVAL_MS;
    connection.on("ping", () => {
        console.debug("Heartbeat received"); 
    });

    connection.onreconnecting((error) => {
        console.log("[SignalR] Entering Reconnecting state");
      if (isMounted) setConnectionStatus("Reconnecting");
      console.warn("[SignalR] Connection lost. Reconnecting...", error);
    });

    connection.onreconnected(() => {
        console.log("[SignalR] Back Online!");
        if (isMounted) setConnectionStatus("Connected"); 
    });
    
    connection.onclose((error) => {
      if (isMounted) setConnectionStatus("Disconnected");
      console.error("[SignalR] Connection closed.", error);
    });

    const handleNewAlert = (raw: any) => {
      if (!isMounted) return;
      try {
        const rawList: AlertMessage[] = Array.isArray(raw) ? raw : [raw];
        const alerts: Alert[] = mapAlertMessagesToAlerts(rawList);
        
         dispatch({ type: "ADD_ALERTS", payload: alerts });
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
          await fetchInitialAlerts();
        }
      } catch (err) {
        if (isMounted) {
          setConnectionStatus("Disconnected");
          console.error("[useAlertsSignalR] Start fail:", err);
          setTimeout(start, RETRY_INTERVAL_MS);
        }
      }
    }

    const fetchInitialAlerts = async () => {
      try {
        const response = await fetch(baseUrl + '/' + ALERTS_ENDPOINT);

        if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
        
        const data: AlertMessage[] = await response.json();
        
        if (isMounted && data.length > 0) {
          console.log(`[useAlertsSignalR] Fetched ${data.length} initial alerts`);
          handleNewAlert(data.sort((a, b)=> a.timestamp - b.timestamp));
        }
      } catch (err) {
        console.error("[useAlertsSignalR] Failed to fetch initial alerts:", err);
      }
    };

    start();

    return () => {
      isMounted = false; // Prevent further state updates
      clearInterval(cleanupInterval);
      connection.off("newAlert", handleNewAlert); // Unregister listener
      connection.stop().catch(console.error);    // Shut down fully
    };
  }, [baseUrl]);

  const alerts = useMemo(() => Array.from(state.alerts.values()), [state.alerts]);
  const activeLocations = useMemo(() => Array.from(state.activeLocations.values()), [state.activeLocations]);

  return { alerts, activeLocations, connectionStatus };
}


