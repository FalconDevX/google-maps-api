import logo from "../../../assets/social-icons/logo.png";
import {Settings} from "lucide-react";
import { useNavigate } from "react-router-dom";
import  "./DemoHeader.css";

const DemoHeader = () => {
  const navigate = useNavigate();
  
  return (
    <div className="demo-header">

        <img src={logo} alt="Logo" className="demo-logo" onClick={() => navigate('/')}/>
        <nav className="navbar">
            <a href="#panel" className="navbar-link">Panel</a>
            <a href="#stats" className="navbar-link">Statystyki</a>
            <a href="#saved" className="navbar-link">Zapisane</a>
            <a href="#settings" className="navbar-link">
              <Settings className="navbar-icon" />
              Ustawienia
            </a>
        </nav>
    </div>
  )
}

export default DemoHeader