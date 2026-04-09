import React, { useState, useMemo } from 'react';
import { useMonitoredSync } from '../hooks/useMonitoredSync';
import { LocationSearch } from './LocationSearch';
import { MonitorToggle } from './MonitorToggle';
import { allIsraelLocations } from '../../shared/utils/locationUtils'; // Moved flattening here
import { alertTypeIconMap } from '../../alerts/services/constants/alertTypeIconMap';
import { AlertType } from '../../shared/model/AlertType';
import type { ActiveAlertLocation } from '../../shared/model/ActiveAlertLocation';
import "./MonitoredLocationsManager.css";
import type { Location } from '../../shared/model/Location';

export const MonitoredManager: React.FC<{ alerts: ActiveAlertLocation[] }> = ({ alerts }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const { mergedLocations, settings, updateSettings } = useMonitoredSync(alerts);

  const suggestions = useMemo(() => {
    if (searchTerm.length < 2) return [];
    return allIsraelLocations
      .filter(loc => loc.id.includes(searchTerm))
      .slice(0, 6);
  }, [searchTerm]);

  const handleAddLocation = (loc: Location) => {
  if (!settings.monitoredLocations.some(m => m.id === loc.name)) {
    updateSettings({
      monitoredLocations: [
        ...settings.monitoredLocations, 
        { id: loc.name, message: '', recievedAt: new Date(), type: AlertType.NO_ALERTS } 
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
          <ul className="sidebar__list-clean">
            {mergedLocations.map((loc) => (
              <li key={loc.id} className={`sidebar__item sidebar__item--stretch ${loc.isLive ? 'sidebar__item--active' : ''}`}>
                <div className="sidebar__item-location">
                  <span className="alert-popup__emoji">{alertTypeIconMap[loc.type] || '🔔'}</span>
                  <span className="sidebar__item-location-text">{loc.id}</span>
                  <MonitorToggle activeAlert={loc} />
                </div>
                <div className="sidebar__item-recieved">
                  {loc.message}
                </div>
              </li>
            ))}
          </ul>
        )}
      </div>
    </aside>
  );
};