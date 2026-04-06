const alertsSseUrl = import.meta.env.VITE_ALERTS_SSE_URL;
console.log("SSE URL:", import.meta.env.VITE_ALERTS_SSE_URL);
if (!alertsSseUrl) throw new Error("Missing VITE_ALERTS_SSE_URL");

export const config = { alertsSseUrl } as const;