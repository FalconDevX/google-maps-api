import "./App.css";
import WelcomeScreenMain from "./Components/WelcomeScreen/WelcomeScreenMain/WelcomeScreenMain";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Demo from "./Components/Demo/Demo";
import DemoSearch from "./Components/Demo/DemoSearch/DemoSearch";

function App() {
  return (
    <BrowserRouter>
      <div className="App">
        <Routes>
          <Route path="/*" element={<WelcomeScreenMain />} />
          <Route path="/demo" element={<Demo />} />
          <Route path="/search" element={<DemoSearch />} />
          <Route path="/settings" element={<div style={{ color: "white" }}>Ustawienia (w budowie)</div>} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
