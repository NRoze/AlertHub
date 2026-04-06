import React, { useMemo } from "react";
import { MapContainer, TileLayer, Marker, Popup } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import type { ActiveAlertLocation } from "../model/ActiveAlertLocation";
import { createAlertIcon } from "../utils/createAlertIcon";
import { alertTypeIconMap } from "../services/constants/alertTypeIconMap";
import "./AlertsMap.css";

// Israel geographic center + sensible zoom
const ISRAEL_CENTER: [number, number] = [31.5, 34.9];
const DEFAULT_ZOOM = 8;

type Props = {
  alerts: ActiveAlertLocation[];
};

export const AlertsMap: React.FC<Props> = ({ alerts }) => {
  return (
    <MapContainer
      center={ISRAEL_CENTER}
      zoom={DEFAULT_ZOOM}
      className="alerts-map"
      zoomControl={true}
    >
      <TileLayer
        url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
      />

      {alerts.map((alert) => (
        <AlertMarker key={alert.id} alert={alert} />
      ))}
    </MapContainer>
  );
};

// Separate component so each marker memoizes its own icon independently
const AlertMarker: React.FC<{ alert: ActiveAlertLocation }> = ({ alert }) => {
  const icon = useMemo(() => createAlertIcon(alert.alertType), [alert.alertType]);

  return (
    <Marker
      position={[alert.location.lat, alert.location.lon]}
      icon={icon}
    >
      <Popup className="alert-popup">
        <div className="alert-popup__header">
          <span className="alert-popup__emoji">{alertTypeIconMap[alert.alertType]}</span>
          <strong className="alert-popup__location">{alert.location.name}</strong>
        </div>
        <div className="alert-popup__message">{alert.message}</div>
        <div className="alert-popup__since">
          Expires {alert.recievedAt.toLocaleTimeString()}
        </div>
      </Popup>
    </Marker>
  );
};
