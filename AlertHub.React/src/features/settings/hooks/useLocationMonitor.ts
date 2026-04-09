import { useState, useMemo, useCallback, useEffect } from 'react';
import { AlertType } from '../../shared/model/AlertType';
import type { ActiveAlertLocation } from '../../shared/model/ActiveAlertLocation';

const STORAGE_KEY = 'monitored_locations_v1';

export const useLocationMonitor = (activeAlerts: ActiveAlertLocation[]) => {
  const [selectedLocations, setSelectedLocations] = useState<ActiveAlertLocation[]>([]);

  // 1. LOAD: Only runs once on mount
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

  // 2. ADD: Manual execution
  const addLocation = useCallback((newEntry: ActiveAlertLocation) => {
    setSelectedLocations((prev) => {
      if (prev.some(m => m.id === newEntry.id)) return prev;
      
      const updated = [...prev, newEntry];
      
      // IMMEDIATE DISK WRITE
      localStorage.setItem(STORAGE_KEY, JSON.stringify(updated));
      console.log("[Storage Write] Saved to disk:", updated);
      
      return updated;
    });
  }, []);

  // 3. UI DISPLAY
  const displayList = useMemo(() => {
    // If this list is empty but selectedLocations has items, the ID find is failing
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