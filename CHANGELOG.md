# Changelog

All notable changes to this project will be documented here.  
Following [Semantic Versioning](https://semver.org/).

---

## [v0.1.0] – 2026-04-04
**First working version**
- Timer-triggered Azure Function publishing alerts
- Redis integration for message passing
- WebApi SSE endpoint exposing real-time updates
- React frontend receiving alerts
- CI workflow configured for GitHub Actions on push
- CD workflow configured for GitHub Actions on new release tag

---

## [v0.1.14] – 2026-04-04
**Bug fixes**
- Fix CD workflow to correctly deploy to Azure

---

## [v0.2.0] – 2026-04-04
**Infrastructure**
- Added Health Check endpoint to WebApi
- Added Health Function to Azure Function App

---

## [v0.3.0] – 2026-04-06
**Features**
- Added a react-leaflet map to the frontend
- Alerts shown for a congurable expiry time on the map
- Including GitHub workflow for React depoly

---

## [v0.5.0] – 2026-04-06
**Features**
- Conditional deployment by repo change since last release tag
- Work around free tier SSE implementation by keepalives
- Fixed azure and asp.net conflicts

---

## [v0.6.0] – 2026-04-08
**Architecture Change**
- Deprecating Redis due to high cost
- Deprecating WebApi for simplicity
- Azure function will use signalR to push updates to the frontend

---

## [v0.7.0] – 2026-04-08
**Architecture Change**
- Function app will use Azure SignalR Service to push updates to the frontend
- Raw messages mapped to location DTO and managed in cache
- Client recieves list of locations and map to active alerts
- Client manage its own expiration for alerts

---

## [v0.8.0] – 2026-04-09
**Features**
- Monitoring locations, saved in local storage
- List of monitored locations + auto complete search
- Toggle button for monitoring

---

## [v0.9.0] – 2026-04-09
**Features**
- Connection status, display Live/Connecting/Reconnecting/Offline
- Conditional CI 

---

## [v0.11.0] – 2026-04-09
**Architecture Change**
- Backend now caches original alert objects
- Frontend displays original alerts on right side bar
- Frontend displays location objects on left side bar

---

