import locationsData from '../data/israelLocations.json';

export const allIsraelLocations = Object.entries(locationsData.areas).flatMap(([areaName, cities]) =>
  Object.entries(cities).map(([cityName, coords]) => ({
    id: cityName,
    name: cityName,
    area: areaName,
    location: { lat: coords.lat, lon: coords.long }
  }))
);