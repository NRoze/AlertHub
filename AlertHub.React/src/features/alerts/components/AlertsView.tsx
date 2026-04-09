import React from "react";
import "./AlertsView.css";
import { AlertsMap } from "./AlertsMap";
import { alertTypeIconMap } from "../services/constants/alertTypeIconMap";
import { MonitorToggle } from "../../settings/components/MonitorToggle";
import { MonitoredManager } from "../../settings/components/MonitoredManager";
import type { ActiveAlertLocation } from "../../shared/model/ActiveAlertLocation";

interface AlertsViewProps {
  alerts: ActiveAlertLocation[];
  connectionStatus: string;
}

/**
 * Top-level alerts feature component.
 * Owns the SSE connection and distributes data to map + sidebar.
 */
export const AlertsView: React.FC<AlertsViewProps> = ({ alerts }) => {

  return (
    <div className="alerts-view">
      <div className="alerts-view__settings">
            <MonitoredManager alerts={alerts}  />
      </div>
      <div className="alerts-view__map">
        <AlertsMap alerts={alerts} />
      </div>

      <aside className="alerts-view__sidebar">
        <h2 className="sidebar__title">
          Active Alerts
          <span className="sidebar__badge">{alerts.length}</span>
        </h2>

        {alerts.length === 0 ? (
          <div className="sidebar__empty">
            <span className="sidebar__empty-icon">✅</span>
            <p>No active alerts</p>
          </div>
        ) : (
          <ul className="sidebar__list">
            {alerts.map((alert) => (
              <li key={alert.id} className="sidebar__item">
                <div className="sidebar__item-location">
                  <span className="alert-popup__emoji">{alertTypeIconMap[alert.type]}</span>
                  {alert.location.name}
                  <MonitorToggle activeAlert={{...alert}}/>
                </div>
                <div className="sidebar__item-recieved">
                  Since {alert.recievedAt.toLocaleTimeString()}
                </div>
              </li>
            ))}
          </ul>
        )}
      </aside>
    </div>
  );
};
