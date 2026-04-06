const alertsSseUrl = import.meta.env.VITE_ALERTS_SSE_URL;
if (!alertsSseUrl) throw new Error("Missing VITE_ALERTS_SSE_URL");

const alertsTtlMs = Number(import.meta.env.VITE_ACTIVE_ALERT_TTL_MS);
if (isNaN(alertsTtlMs)) {
  throw new Error("Invalid VITE_ACTIVE_ALERT_TTL_MS, must be a number");
}

export const config = { alertsSseUrl, alertsTtlMs } as const; 