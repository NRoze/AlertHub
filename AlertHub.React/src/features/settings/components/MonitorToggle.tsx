import React from 'react';
import "./styles/MonitorToggle.css"
import { useSettings } from '../context/SettingsContext';
import type { MonitoredLocation } from '../model/MonitoredLocation.ts';

interface MonitorToggleProps {
  activeAlert: MonitoredLocation;
}

export const MonitorToggle: React.FC<MonitorToggleProps> = ({ activeAlert }) => {
  const { settings, updateSettings } = useSettings();
  const isMonitored = settings.monitoredLocations.some(loc => loc.name === activeAlert.name);

  const toggleMonitor = (e: React.MouseEvent) => {
    e.stopPropagation();

    const newLocations = isMonitored
      ? settings.monitoredLocations.filter(loc => loc.name !== activeAlert.name)
      : [...settings.monitoredLocations, {
          id: activeAlert.id,
          name: activeAlert.name,
          type: activeAlert.type,
          receivedAt: activeAlert.receivedAt || new Date(),
          message: activeAlert.message || ''
        }];

    updateSettings({ monitoredLocations: newLocations });
  };

  return (
    <button 
      onClick={toggleMonitor}
      className={`monitor-toggle ${isMonitored ? 'is-monitored' : ''}`}
      title={isMonitored ? "Stop monitoring" : "Monitor this location"}
      aria-label="Toggle monitoring"
    >
      {isMonitored ? '👁️' : '👁️‍🗨️'}
    </button>
  );
};