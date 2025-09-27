import React from "react";
import { useNavigate } from "react-router-dom"
import "./RegistrationPanel.css"

const RegistrationPanel = () => {
  const navigate = useNavigate();
  const [username, setUsername] = React.useState("");
  const [email, setEmail] = React.useState("");
  const [password, setPassword] = React.useState("");
  const [confirmPassword, setConfirmPassword] = React.useState("");

  const handleRegistration = async () => {
    try {
      const response = await fetch("http://34.56.66.163/api/Users/register", {
        method: "POST",
        headers: {"Content-Type": "application/json"},
        body: JSON.stringify({ username, email, password })
      });

      if (response.ok) {
        const data = await response.json();
        console.log("Registration successful:", data);
      }
      else{
        const errorText = await response.text();
        console.error("Registration failed:", response.status, errorText);
      }
    } catch (error) {
      console.error("Registration failed:", error);
    }
  }

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
        
        <button className="login-btn" onClick={handleRegistration}>
          Zarejestruj się
        </button>
        
        <div className="login-link">
          <span className="login-text">Masz już konto? </span>
          <a className="login-text-link" onClick={() => navigate("/")}>
            Zaloguj się
          </a>
        </div>
      </div>
    </div>
  );
};

export default RegistrationPanel;