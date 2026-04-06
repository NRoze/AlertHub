const alertsSseUrl = import.meta.env.VITE_ALERTS_SSE_URL;
if (!alertsSseUrl) throw new Error("Missing VITE_ALERTS_SSE_URL");

const alertsTtlMs = import.meta.env.VITE_ACTIVE_ALERT_TTL_MS;
if (!alertsTtlMs) throw new Error("Missing VITE_ACTIVE_ALERT_TTL_MS");

export const config = { alertsSseUrl, alertsTtlMs } as const;