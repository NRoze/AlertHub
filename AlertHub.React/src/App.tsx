import { AlertsView } from "./features/alerts/components/AlertsView";
import { SettingsProvider } from "./features/settings/context/SettingsContext";

function App() {
  return (
    <div className="app">
      <header className="app-header">
        <h1 className="app-header__title">🛡️ AlertHub</h1>
        <span className="app-header__subtitle">Israel Real-Time Alert Map</span>
      </header>
      <main className="app-main">
        <SettingsProvider>
          <div className="app">
            <AlertsView />
          </div>
        </SettingsProvider>
      </main>
    </div>
  );
}

export default App;