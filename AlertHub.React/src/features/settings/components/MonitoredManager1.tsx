import { useMemo, useState } from "react";
import type { ActiveAlertLocation } from "../../shared/model/ActiveAlertLocation";
import { useLocationMonitor } from "../hooks/useLocationMonitor";
import { LocationSearch } from "./LocationSearch";
import { MonitorToggle } from "./MonitorToggle";
import { allIsraelLocations } from '../../shared/utils/locationUtils'; // Moved flattening here
import type { Location } from "../../shared/model/Location";
import { AlertType } from "../../shared/model/AlertType";
import { getTitleByType } from "../../alerts/services/constants/alertTitleMap";
import { alertTypeIconMap } from "../../alerts/services/constants/alertTypeIconMap";
import "./MonitoredLocationsManager.css";

export const MonitoredManager: React.FC<{ activeAlerts: ActiveAlertLocation[] }> = ({ activeAlerts }) => {
  // Use addLocation instead of setSelectedLocations for manual persistence
  const { displayList, addLocation } = useLocationMonitor(activeAlerts);
  const [searchTerm, setSearchTerm] = useState('');
  
  const suggestions = useMemo(() => {
    if (searchTerm.length < 2) return [];
    return allIsraelLocations
      .filter(loc => loc.id.toLowerCase().includes(searchTerm.toLowerCase()))
      .slice(0, 6);
  }, [searchTerm]);
  
  const handleAddLocation = (loc: Location) => {
    // Construct the entry
    const newEntry: ActiveAlertLocation = {
      id: loc.name,
      location: loc,
      type: AlertType.NO_ALERTS,
      message: getTitleByType(AlertType.NO_ALERTS),
      recievedAt: new Date(),
      expiresAt: new Date(),
    };

    // Call the stable add function
    addLocation(newEntry);
    setSearchTerm('');
  };

  return (
    <aside className="alerts-view__sidebar settings-sidebar">
      <h2 className="sidebar__title">
        Monitored Locations
        <span className="sidebar__badge">{displayList.length}</span>
      </h2>

      <LocationSearch 
        searchTerm={searchTerm} 
        setSearchTerm={setSearchTerm} 
        suggestions={suggestions} 
        onSelect={handleAddLocation} 
      />

      <div className="sidebar__list">
        {displayList.length === 0 ? (
          <div className="sidebar__empty">
            <span className="sidebar__empty-icon">👁️‍🗨️</span>
          </div>
        ) : (
          <ul className="sidebar__list-clean">
            {displayList.map((loc) => (
              <li key={loc.id} className="sidebar__item sidebar__item--stretch">
                <div className="sidebar__item-location">
                  <span className="alert-popup__emoji">{alertTypeIconMap[loc.type] || ''}</span>
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