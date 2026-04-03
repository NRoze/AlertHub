import { useAlertsSSE } from "./hooks/useAlertsSSE";

function App() {
  const alerts = useAlertsSSE();

  return (
    <div style={{ padding: "2rem" }}>
      <h1>AlertHub 🚨</h1>
      <p>Listening for alerts...</p>
      <ul>
        {alerts?.map((alert, index) => (
          <li key={index}>{alert}</li>
        ))}
      </ul>
    </div>
  );
}

export default App;