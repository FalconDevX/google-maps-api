import React from "react";
import { useNavigate } from "react-router-dom"
import "./RegistrationPanel.css"

const RegistrationPanel = () => {
  const navigate = useNavigate();
  return (
    <div className="registration-panel">
      <div className="registration-card">
        <h2 className="registration-title">Utwórz konto</h2>
        <div className="form-fields">
          <div className="input-group">
            <label className="input-label">Username</label>
            <input 
              type="text" 
              className="form-input" 
              placeholder="np. XYZ"
            />
          </div>

          <div className="input-group">
            <label className="input-label">Email</label>
            <input 
              type="email" 
              className="form-input" 
              placeholder="np. abc@xyz.dev"
            />
          </div>
          
          <div className="input-group">
            <label className="input-label">Hasło</label>
            <input 
              type="password" 
              className="form-input" 
              placeholder="••••••••"
            />
          </div>

          <div className="input-group">
            <label className="input-label">Potwierdź hasło</label>
            <input 
              type="password" 
              className="form-input" 
              placeholder="••••••••"
            />
          </div>
        </div>
        
        <button className="login-btn">Zarejestruj się</button>
        
        <div className="login-link">
          <span className="login-text">Masz już konto? </span>
          <a href="#" className="login-link-text" onClick={() => navigate('/')}>Zaloguj się</a>
        </div>
      </div>
    </div>
  );
};

export default RegistrationPanel;