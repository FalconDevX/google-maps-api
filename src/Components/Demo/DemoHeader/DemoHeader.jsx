import logo from "../../../assets/social-icons/logo.png";
import { Settings } from "lucide-react";
import { NavLink, useNavigate } from "react-router-dom";
import "./DemoHeader.css";

const DemoHeader = () => {
  const navigate = useNavigate();

  return (
    <div className="demo-header">
      <img
        src={logo}
        alt="Logo"
        className="demo-logo"
        onClick={() => navigate("/")}
      />
      <nav className="navbar">
        <NavLink to="/search" className="navbar-link">
          Przeglądaj
        </NavLink>
        <NavLink to="/demo" className="navbar-link">
          Panel
        </NavLink>
        <NavLink to="/settings" className="navbar-link">
          <Settings className="navbar-icon" />
          Ustawienia
        </NavLink>
      </nav>
    </div>
  );
};

export default DemoHeader;
