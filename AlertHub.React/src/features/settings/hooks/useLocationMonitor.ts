import { useState, useMemo, useCallback, useEffect } from 'react';
import { AlertType } from '../../shared/model/AlertType';
import type { ActiveAlertLocation } from '../../shared/model/ActiveAlertLocation';

const STORAGE_KEY = 'monitored_locations_v1';

export const useLocationMonitor = (activeAlerts: ActiveAlertLocation[]) => {
  const [selectedLocations, setSelectedLocations] = useState<ActiveAlertLocation[]>([]);

  useEffect(() => {
    const saved = localStorage.getItem(STORAGE_KEY);
    console.log("[Storage Read] Raw data from disk:", saved);
    
    if (saved && saved !== "null") {
      try {
        const parsed = JSON.parse(saved);
        setSelectedLocations(parsed);
      } catch (e) {
        console.error("[Storage Read] Parse error:", e);
      }
    }
  }, []);

  const addLocation = useCallback((newEntry: ActiveAlertLocation) => {
    setSelectedLocations((prev) => {
      if (prev.some(m => m.id === newEntry.id)) return prev;
      
      const updated = [...prev, newEntry];
      
      localStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
      console.log("[Storage Write] Saved to disk:", updated);
      
      return updated;
    });
  }, []);

  const displayList = useMemo(() => {
    return selectedLocations.map((storedLoc) => {
      const active = activeAlerts.find((a) => String(a.id) === String(storedLoc.id));
      return active || {
        ...storedLoc,
        type: AlertType.NO_ALERTS,
        message: "No active alerts"
      };
    });
  }, [activeAlerts, selectedLocations]);

  return { displayList, addLocation };
};