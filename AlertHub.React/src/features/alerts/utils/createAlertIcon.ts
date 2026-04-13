import L from "leaflet";
import { alertTypeLineColorMap, alertTypeIconMap } from "../services/constants/alertTypeIconMap";
import { AlertType } from "../../shared/model/AlertType";

export const createAlertIcon = (type: AlertType) => {
  const emoji = alertTypeIconMap[type];
  const color = alertTypeLineColorMap[type];
  const html = `
    <div class="emoji-marker-container">
        <div class="emoji-marker__emoji">${emoji}</div>
        <div class="emoji-marker__line" style="background-color: ${color}"></div>
    </div>
  `;

  return L.divIcon({
    html,
    className: "leaflet-marker-invisible",
    iconSize: [20, 30],       // NEW: Width 20, Height 30
    iconAnchor: [10, 30],     // NEW: Horizontal center (10), Bottom (30)
    popupAnchor: [0, -30]     // NEW: Popup opens at the top of the 30px box
  });
};