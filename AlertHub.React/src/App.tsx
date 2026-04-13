import { config } from "./config/config";
import { AlertsView } from "./features/alerts/components/AlertsView";
import { useAlertsSignalR } from "./features/alerts/hooks/useAlertsSignalR";
import { SettingsProvider } from "./features/settings/context/SettingsContext";
import "./App.css";

const statusIcons = {
    Connected: { icon: "🟢", label: "Live" },
    Connecting: { icon: "🟡", label: "Connecting" },
    Reconnecting: { icon: "🟠", label: "Reconnecting" },
    Disconnected: { icon: "🔴", label: "Offline" },
  };

function App() {
  const { alerts, activeLocations, connectionStatus } = useAlertsSignalR(config.alertsApiBase);

  return (
    <div className="app">
      <header className="app-header">
        <h1 className="app-header__title">🛡️ AlertHub</h1>
        <div className={`app-header__status status--${connectionStatus.toLowerCase()}`}>
          <span className="status-icon">{statusIcons[connectionStatus].icon}</span>
          <span className="status-text">{statusIcons[connectionStatus].label}</span>
        </div>
      </header>
      <main className="app-main">
        <SettingsProvider>
          <div className="app-view">
            <AlertsView alerts={alerts} activeLocations={activeLocations} connectionStatus={connectionStatus}/>
          </div>
        </SettingsProvider>
      </main>
    </div>
  );
}

export default App;