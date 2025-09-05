import "./App.css";
import WelcomeScreenMain from "./Components/WelcomeScreen/WelcomeScreenMain/WelcomeScreenMain";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Demo from "./Components/Demo/Demo";

function App() {
  return (
    <BrowserRouter>
      <div className="App">
        <Routes>
          <Route path="/" element={<WelcomeScreenMain />} />
          <Route path="/demo" element={<Demo />} />
        </Routes>
      </div>
    </BrowserRouter>
  );
}

export default App;
