import { useEffect, useState } from "react";

export function useAlertsSSE() {
  const [alerts, setAlerts] = useState<string[]>([]); // ✅ starts as empty array

  useEffect(() => {
    console.log("Hook initialized");
    const url = "https://localhost:7154/api/alerts/sse";
    const eventSource = new EventSource(url);

    eventSource.onmessage = (event) => {
    console.log("Got SSE message:", event.data);
      setAlerts((prev) => [...prev, JSON.parse(event.data)]);
    };

    eventSource.onerror = (error) => {
        console.log("SSE error event:", error);
      console.error("SSE error:", error);
      eventSource.close();
    };

    return () => eventSource.close();
  }, []);

  return alerts;
}