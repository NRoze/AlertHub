# 🛡️ AlertHub

AlertHub is a real-time, event-driven web application and backend suite that monitors, processes, and displays live emergency alerts (such as rockets, earthquakes, etc.) in Israel. It interfaces with the Home Front Command (Pikud HaOref) APIs to broadcast geographically mapped alerts via WebSockets to a React frontend.

## 🏗️ Solution Architecture

The solution uses a streamlined, cloud-native architecture to ensure separation of concerns, high scalability, and cost-efficiency:

- **`AlertHub.Api` (Azure Functions)**: The core serverless worker service. It contains background functions that continuously poll the Pikud HaOref API for new alerts. It processes incoming data, handles the caching, and broadcasts real-time events directly to clients via SignalR.
- **`AlertHub.React` (Frontend)**: A modern Single Page Application built with Vite, React, and TypeScript. It utilizes SignalR to maintain a live WebSocket connection to the backend and renders alerts on an interactive map using `react-leaflet`. It also includes a robust location-monitoring and management sidebar.
- **`AlertHub.Tests`**: Comprehensive test suite encompassing unit and integration coverage.

### 📉 Architecture Evolution (Historical Note)
Previously, the solution included an `AlertHub.WebApi` component acting as a middle-tier service, along with a Redis cluster for distributed caching and an event bus, and auxiliary `Shared` and `Infrastructure` boundaries. These dependencies were deliberately **deprecated and removed** to drastically reduce cloud hosting costs and simplify the codebase, shifting the entire backend to a pure Serverless Azure Functions model.

## 🚀 Getting Started

### Prerequisites
- [.NET 8+ SDK](https://dotnet.microsoft.com/)
- [Node.js 18+](https://nodejs.org/)
- Azure Storage Emulator / Azurite (for local Azure Functions development)

### Running Locally

1. **Start Azurite**
   Make sure Azurite is running locally to emulate Azure Storage for the Functions runtime.
   
2. **Backend Services**
   - Review connections in `AlertHub.Api/appsettings.function.json` (or `local.settings.json`).
   - Navigate to the `AlertHub.Api` directory and start the functions app:
     ```bash
     func start
     ```

3. **Start the Frontend**
   - Navigate to the frontend directory: 
     ```bash
     cd AlertHub.React
     ```
   - Install dependencies: 
     ```bash
     npm install
     ```
   - Start the Vite development server: 
     ```bash
     npm run dev
     ```
   - Open your browser at `http://localhost:5173`

---

## 🧪 Simulation Mode (Testing)

For development and UI feature-testing, you do not need to wait for real-life emergencies. The Azure Functions app (`AlertHub.Api`) supports a powerful built-in **simulation mode**.

To inject mock alerts into the system continuously, adjust the configuration in `appsettings.function.json` (or `local.settings.json` locally). Set the `UseSimulatedAlerts` flag to `true` under the `PikudPoller` options:

```json
{
  "PikudPoller": {
    "UseSimulatedAlerts": true
  }
}
```

When this flag is active, the application swaps the live polling client for the `SimulatedPikudPollerService`. This immediately begins emitting simulated emergency payloads into the system pipeline, allowing you to seamlessly test the React map markers, expiry timers, and location-monitoring alerts locally without contacting the live API.

## 📄 License
Refer to the `LICENSE.txt` file in the root directory for licensing details.