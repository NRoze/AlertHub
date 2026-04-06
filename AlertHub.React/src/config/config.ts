const alertsSseUrl = import.meta.env.VITE_ALERTS_SSE_URL;
if (!alertsSseUrl) throw new Error("Missing VITE_ALERTS_SSE_URL");

const alertsTtlMs = import.meta.env.ACTIVE_ALERT_TTL_MS;
if (!alertsTtlMs) throw new Error("Missing ACTIVE_ALERT_TTL_MS");

export const config = { alertsSseUrl, alertsTtlMs } as const;