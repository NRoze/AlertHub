import { useEffect, useState } from "react";
import type { AlertMessage } from "../types/AlertMessage";

export function useAlertsSSE() {
  const [alerts, setAlerts] = useState<string[]>([]); // ✅ starts as empty array

  useEffect(() => {
    console.log("Hook initialized");
    //const url = "https://localhost:7154/api/alerts/sse";
    const url = "https://alert-hub-webapi-hufjcrbcfgd3engk.israelcentral-01.azurewebsites.net/api/alerts/sse";
    const eventSource = new EventSource(url);

   eventSource.onmessage = (event) => {
      try {
        let parsed = JSON.parse(event.data);
        
        if (typeof parsed === 'string') {
            parsed = JSON.parse(parsed);
        }
        
        const message: AlertMessage = parsed;
        
        if (message && typeof message.title === 'string') {
            setAlerts((prev) => [...prev, message.title]);
        } else {
            console.warn("Title is missing or invalid:", message);
        }
      } catch (error) {
          console.error("Parse error:", error);
      }
    };

    eventSource.onerror = (error) => {
      console.error("SSE error:", error);
      eventSource.close();
    };

    return () => eventSource.close();
  }, []);

  return alerts;
}