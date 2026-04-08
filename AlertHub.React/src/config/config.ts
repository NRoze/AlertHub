const alertsApiBase = import.meta.env.VITE_ALERTS_API_BASE_URL;
if (!alertsApiBase) throw new Error("Missing VITE_ALERTS_API_BASE_URL");

const alertsTtlMs = Number(import.meta.env.VITE_ACTIVE_ALERT_TTL_MS);
if (isNaN(alertsTtlMs)) {
  throw new Error("Invalid VITE_ACTIVE_ALERT_TTL_MS, must be a number");
}


export const config = { alertsApiBase, alertsTtlMs } as const;