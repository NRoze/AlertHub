/**
 * Application configuration.
 *
 * Values are read from Vite environment variables (VITE_* prefix).
 * For local development, set them in a `.env.local` file (gitignored).
 * For Azure Static Web Apps, set them as Application Settings in the portal —
 * they are injected at build time by the CI pipeline.
 */
export const config = {
  alertsSseUrl:
    import.meta.env.VITE_ALERTS_SSE_URL ??
    "https://localhost:7154/api/alerts/sse",
    //"https://alert-hub-webapi-hufjcrbcfgd3engk.israelcentral-01.azurewebsites.net/api/alerts/sse",
} as const;
