import locationJsonRaw from "../../data/israelLocations.json";
import type { Location } from "../../model/Location";

type LocationCoordinates = { lat: number; long: number };
type AreaLocations = Record<string, LocationCoordinates>;
type IsraelLocationsJson = { areas: Record<string, AreaLocations> };

const locationJson = locationJsonRaw as IsraelLocationsJson;

export const locationMap: Map<string, Location> = new Map();

Object.entries(locationJson.areas).forEach(([areaName, areaLocations]) => {
  Object.entries(areaLocations).forEach(([cityName, coords]) => {
    locationMap.set(cityName, {
      name: cityName,
      lat: coords.lat,
      lon: coords.long,
      area: areaName,
    });
  });
});
