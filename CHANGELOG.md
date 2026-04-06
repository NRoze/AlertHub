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
***Infrastructure**
- Added Health Check endpoint to WebApi
- Added Health Function to Azure Function App

---

## [v0.3.0] – 2026-04-06
***Features**
- Added a react-leaflet map to the frontend
- Alerts shown for a congurable expiry time on the map
- Including GitHub workflow for React depoly

---