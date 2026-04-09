import React, { createContext, useContext, useEffect, useState } from 'react';
import { type UserSettings, DEFAULT_SETTINGS } from '../types/settings.types';

interface SettingsContextType {
  settings: UserSettings;
  updateSettings: (newSettings: Partial<UserSettings>) => void;
  resetSettings: () => void;
}

const SettingsContext = createContext<SettingsContextType | undefined>(undefined);

const STORAGE_KEY = 'alert_hub_user_settings';

export const SettingsProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  // Helper to turn strings back into Date objects
  const hydrateSettings = (raw: any): UserSettings => {
    return {
      ...raw,
      monitoredLocations: (raw.monitoredLocations || []).map((loc: any) => ({
        ...loc,
        recievedAt: new Date(loc.recievedAt),
      })),
    };
  };
  
  const [settings, setSettings] = useState<UserSettings>(() => {
    try {
      const saved = localStorage.getItem(STORAGE_KEY);
      if (!saved) return DEFAULT_SETTINGS;
      
      const parsed = JSON.parse(saved);
      const hydrated = hydrateSettings(parsed);

      if (hydrated.version < DEFAULT_SETTINGS.version) {
        return { ...DEFAULT_SETTINGS, ...hydrated, version: DEFAULT_SETTINGS.version };
      }

      return hydrated;
    } catch (e) {
      console.error("Failed to load settings", e);
      return DEFAULT_SETTINGS;
    }
  });

  useEffect(() => {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(settings));
  }, [settings]);

  // Handle cross-tab sync
  useEffect(() => {
    const handleStorage = (e: StorageEvent) => {
      if (e.key === STORAGE_KEY && e.newValue) {
        const raw = JSON.parse(e.newValue);
        setSettings(hydrateSettings(raw));
      }
    };
    window.addEventListener('storage', handleStorage);
    return () => window.removeEventListener('storage', handleStorage);
  }, []);

  const updateSettings = (updates: Partial<UserSettings>) => {
    setSettings(prev => ({ ...prev, ...updates }));
  };

  const resetSettings = () => {
    localStorage.removeItem(STORAGE_KEY);
    setSettings(DEFAULT_SETTINGS);
  };

  return (
    <SettingsContext.Provider value={{ settings, updateSettings, resetSettings }}>
      {children}
    </SettingsContext.Provider>
  );
};

export const useSettings = () => {
  const context = useContext(SettingsContext);
  if (!context) throw new Error('useSettings must be used within a SettingsProvider');
  return context;
};