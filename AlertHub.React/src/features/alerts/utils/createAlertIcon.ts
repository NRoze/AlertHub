import L from "leaflet";
import { alertTypeLineColorMap, alertTypeIconMap } from "../services/constants/alertTypeIconMap";
import { AlertType } from "../model/AlertType";

export const createAlertIcon = (type: AlertType) => {
  const emoji = alertTypeIconMap[type];
  const color = alertTypeLineColorMap[type];
  const html = `
    <div class="emoji-marker">
      <div>${emoji}</div>
      <div class="emoji-marker__line" style="color: ${color}"></div>
    </div>
  `;

  return L.divIcon({
    html,
    className: "",
    iconSize: [24, 39],      // approximate
    iconAnchor: [12, 39],    // point of the line touches the map location
    popupAnchor: [0, -35]    // popup above emoji
  });
};