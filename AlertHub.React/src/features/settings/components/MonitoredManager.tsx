import React, { useState, useMemo } from 'react';
import { useMonitoredSync } from '../hooks/useMonitoredSync';
import { LocationSearch } from './LocationSearch';
import { MonitorToggle } from './MonitorToggle';
import { allIsraelLocations } from '../../shared/utils/locationUtils'; // Moved flattening here
import { alertTypeIconMap, alertTypeMessageMap } from '../../alerts/services/constants/alertTypeIconMap';
import { AlertType } from '../../shared/model/AlertType';
import type { ActiveAlertLocation } from '../../shared/model/ActiveAlertLocation';
import "./styles/MonitoredLocationsManager.css";
import type { Location } from '../../shared/model/Location';

export const MonitoredManager: React.FC<{ alerts: ActiveAlertLocation[] }> = ({ alerts }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const { mergedLocations, settings, updateSettings } = useMonitoredSync(alerts);

  const suggestions = useMemo(() => {
    if (searchTerm.length < 2) return [];
    return allIsraelLocations
      .filter(loc => loc.name.includes(searchTerm))
      .slice(0, 6);
  }, [searchTerm]);

  const handleAddLocation = (loc: Location) => {
  if (!settings.monitoredLocations.some(m => m.name === loc.name)) {
    updateSettings({
      monitoredLocations: [
        ...settings.monitoredLocations, 
        { id: loc.name, name: loc.name, message: '', receivedAt: new Date(), type: AlertType.NO_ALERTS } 
      ]
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

      <LocationSearch 
        searchTerm={searchTerm} 
        setSearchTerm={setSearchTerm} 
        suggestions={suggestions} 
        onSelect={handleAddLocation} 
      />

      <div className="sidebar__list">
        {mergedLocations.length === 0 ? (
          <div className="sidebar__empty"><span className="sidebar__empty-icon">👁️‍🗨️</span></div>
        ) : (
          <ul className="sidebar-monitor__list-clean">
            {mergedLocations.map((loc) => (
              <li key={loc.id} 
                  className={`sidebar-monitor__item sidebar-monitor__item--stretch 
                  ${loc.isLive ? 'sidebar__item--active' : ''}`}>
                <div className="sidebar-monitor__item-location">
                  <span className="sidebar__item-location-emoji">
                    {alertTypeIconMap[loc.type] || '🔔'}
                  </span>
                  <span className="sidebar-monitor__item-location-text">{loc.name}</span>
                  <MonitorToggle activeAlert={loc} />
                </div>
                <div className="sidebar-monitor__item-message">
                  <span className="sidebar-monitor__item-message">{alertTypeMessageMap[loc.type]}</span>
                  <span className="sidebar-monitor__item-date">
                    {loc.receivedAt.toLocaleTimeString(
                      [], { hour: '2-digit', minute: '2-digit', hour12: false })}
                  </span>
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </aside>
  );
};