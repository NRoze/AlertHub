import React, { useMemo } from "react";
import "./styles/AlertsView.css";
import { AlertsMap } from "./AlertsMap";
import { alertTypeIconMap, alertTypeMessageMap } from "../services/constants/alertTypeIconMap";
import { MonitoredManager } from "../../settings/components/MonitoredManager";
import type { Alert } from "../model/Alert";
import type { ActiveAlertLocation } from "../../shared/model/ActiveAlertLocation";

interface AlertsViewProps {
  alerts: Alert[];
  activeLocations: ActiveAlertLocation[];
  connectionStatus: string;
}

/**
 * Top-level alerts feature component.
 * Owns the SSE connection and distributes data to map + sidebar.
 */
export const AlertsView: React.FC<AlertsViewProps> = ({ alerts, activeLocations}) => {
  const sortedAlerts = useMemo(() => {
    return [...alerts].sort((a, b) => b.receivedAt.getTime() - a.receivedAt.getTime());
  }, [alerts]);

  return (
    <div className="alerts-view">
      <div className="alerts-view__settings">
        <MonitoredManager alerts={activeLocations}  />
      </div>
      <div className="alerts-view__map">
        <AlertsMap alerts={activeLocations} />
      </div>

      <aside className="alerts-view__sidebar">
        <h2 className="sidebar__title">
          Recent Alerts
          <span className="sidebar__badge">{sortedAlerts.length}</span>
        </h2>

        {sortedAlerts.length === 0 ? (
          <div className="sidebar__empty">
            <span className="sidebar__empty-icon">✅</span>
            <p>No active alerts</p>
          </div>
        ) : (
          <ul className="sidebar__list">
            {sortedAlerts.map((alert) => (
              <li key={alert.id} className="sidebar__item">
                <div className="sidebar__item-location">
                  <span className="sidebar__item-emoji">{alertTypeIconMap[alert.type]}</span>
                  <span className="sidebar__item-message">{alertTypeMessageMap[alert.type]}</span>
                  <div className="sidebar__item-recieved">{alert.receivedAt.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: false })}</div>
                </div>
                <div className="sidebar__item-locations">
                  {alert.locations.map((loc, i) => (
                    <span key={`${alert.id}-${loc.name}`}>
                      {loc.name}
                      {i < alert.locations.length - 1 ? ", " : ""}
                    </span>
                  ))}
                </div>
              </li>
            ))}
          </ul>
        )}
      </aside>
    </div>
  );
};
