import { useEffect, useReducer } from "react";
import type { AlertMessage } from "../model/AlertMessage";
import { normalizeAlert } from "../services/normalizeAlert";
import { alertsReducer, initialAlertsState } from "../store/alertsReducer";
import type { ActiveAlertLocation } from "../model/ActiveAlertLocation";

const CLEAN_INTERVAL_MS = 5_000;

/**
 * Parses SSE message data.
 */
function parseSSEData(raw: string): AlertMessage {
  const first = JSON.parse(raw);
  return typeof first === "string" ? JSON.parse(first) : first;
}

/**
 * Connects to a Server-Sent Events stream at `url` and maintains a live map
 * of active alert locations, expiring stale entries every 5 seconds.
 */
export function useAlertsSSE(url: string): ActiveAlertLocation[] {
  const [state, dispatch] = useReducer(alertsReducer, initialAlertsState);

  useEffect(() => {
    const eventSource = new EventSource(url);

    const cleanupInterval = setInterval(() => {
      dispatch({ type: "CLEAN_EXPIRED", now: Date.now() });
    }, CLEAN_INTERVAL_MS);

    eventSource.onmessage = (event) => {
      try {
        const raw = parseSSEData(event.data);
        const alert = normalizeAlert(raw);
        dispatch({ type: "ADD_ALERT", payload: alert });
      } catch (err) {
        console.error("[useAlertsSSE] Failed to parse SSE message:", err);
      }
    };

    eventSource.onerror = (err) => {
      // EventSource will auto-reconnect; log for visibility only.
      console.error("[useAlertsSSE] SSE connection error:", err);
    };

    return () => {
      eventSource.close();
      clearInterval(cleanupInterval);
    };
  }, [url]);

  return Array.from(state.alerts.values());
}