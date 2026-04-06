import { AlertsView } from "./features/alerts/components/AlertsView";

function App() {
  return (
    <div className="app">
      <header className="app-header">
        <h1 className="app-header__title">🛡️ AlertHub</h1>
        <span className="app-header__subtitle">Israel Real-Time Alert Map</span>
      </header>
      <main className="app-main">
        <AlertsView />
      </main>
    </div>
  );
}

export default App;