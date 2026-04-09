import React, { useState, useMemo, useEffect } from 'react';
import { useSettings } from '../context/SettingsContext';
import locationsData from '../../shared/data/israelLocations.json';
import { MonitorToggle } from './MonitorToggle';
import { AlertType } from '../../shared/model/AlertType';
import "./MonitoredLocationsManager.css";
import { alertTypeIconMap } from '../../alerts/services/constants/alertTypeIconMap';
import type { ActiveAlertLocation } from '../../shared/model/ActiveAlertLocation';
import { getTitleByType } from '../../alerts/services/constants/alertTitleMap';
import { getTimeoutByType } from '../types/settings.types';

interface Props {
  alerts: ActiveAlertLocation[];
}

export const MonitoredLocationsManager: React.FC<Props> = ({ alerts }) => {
    const { settings, updateSettings } = useSettings();
    const [searchTerm, setSearchTerm] = useState('');

    useEffect(() => {
        let hasChanges = false;
        
        const updatedMonitored = settings.monitoredLocations.map(monitored => {
            const activeMatch = alerts.find(a => a.id === monitored.id);
            
            if (activeMatch && (monitored.type !== activeMatch.type || 
                                monitored.message !== activeMatch.message)) {
                hasChanges = true;
                
                return { 
                    ...monitored, 
                    type: activeMatch.type, 
                    message: activeMatch.message 
                };
            }
            return monitored;
        });

        if (hasChanges) {
            updateSettings({ monitoredLocations: updatedMonitored });
        }
    }, [alerts, settings.monitoredLocations, updateSettings]);

  // 1. Flatten locations for the search autocomplete
  const allLocations = useMemo(() => 
    Object.entries(locationsData.areas).flatMap(([areaName, cities]) =>
      Object.entries(cities).map(([cityName, coords]) => ({
        id: cityName, 
        name: cityName,
        area: areaName,
        location: { lat: coords.lat, lon: coords.long }
      }))
    ), []);

    const mergedLocations = useMemo(() => {
        return settings.monitoredLocations.map(monitored => ({
            ...monitored, 
            isLive: alerts.some(a => a.id === monitored.id)
        }));
    }, [settings.monitoredLocations, alerts]);
  

  const suggestions = useMemo(() => {
    if (searchTerm.length < 2) return [];
    return allLocations
      .filter(loc => loc.id.includes(searchTerm))
      .slice(0, 6);
  }, [searchTerm, allLocations]);

  const handleAddLocation = (loc: any) => {
    if (!settings.monitoredLocations.some(m => m.id === loc.id)) {
        
      updateSettings({
        monitoredLocations: [...settings.monitoredLocations, {
          id: loc.id,
          message: getTitleByType(AlertType.NO_ALERTS),
          type: AlertType.NO_ALERTS,
          recievedAt: new Date(),
        }]
      });
    }
    setSearchTerm('');
  };

  return (
    <aside className="alerts-view__sidebar settings-sidebar">
      <h2 className="sidebar__title">
        Monitored Locations
        <span className="sidebar__badge">{settings.monitoredLocations.length}</span>
      </h2>

      <div className="sidebar__search-container">
        <input
          type="text"
          className="sidebar__search-input"
          placeholder="Search..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
        />
        {suggestions.length > 0 && (
          <ul className="sidebar__suggestions">
            {suggestions.map(loc => (
              <li key={loc.id} onClick={() => handleAddLocation(loc)}>
                <span>{loc.name}</span>
              </li>
            ))}
          </ul>
        )}
      </div>

      <div className="sidebar__list">
        {settings.monitoredLocations.length === 0 ? (
          <div className="sidebar__empty">
            <span className="sidebar__empty-icon">👁️‍🗨️</span>
            <p></p>
          </div>
        ) : (
          <ul className="sidebar__list-clean">
            {mergedLocations.map((loc) => (
              <li key={loc.id} className={`sidebar__item sidebar__item--stretch ${loc.isLive ? 'sidebar__item--active' : ''}`}>
                <div className="sidebar__item-location">
                  <span className="alert-popup__emoji">{alertTypeIconMap[loc.type] || ''}</span>
                  <span className="sidebar__item-location-text">{loc.id}</span>
                  <MonitorToggle activeAlert={loc} />
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </aside>
  );
};
