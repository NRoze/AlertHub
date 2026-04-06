/**
 * Centralized app configuration.
 * Reads from Vite environment variables.
 */
function requireEnv(name: string): string {
  const value = import.meta.env[name];
  
  if (!value) {
    throw new Error(`Missing environment variable: ${name}`);
  }

  return value;
}

export const config = {
  alertsSseUrl: requireEnv("VITE_ALERTS_SSE_URL"),
} as const;