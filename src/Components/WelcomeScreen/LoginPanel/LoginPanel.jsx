import React from "react";
import "./LoginPanel.css";
import googleIcon from "../../../assets/social-icons/google-icon.webp";
import facebookIcon from "../../../assets/social-icons/facebook-icon.webp";
import githubIcon from "../../../assets/social-icons/github-icon.png";
import { useNavigate } from "react-router-dom"

const LoginPanel = () => {
  const navigate = useNavigate();

  const [email, setEmail] = React.useState("");
  const [password, setPassword] = React.useState("");

  const handleLogin = async () => {
    try {
      const response = await fetch("http://34.56.66.163/api/Users/login", {
        method: "POST",
        headers: {"Content-Type": "application/json" },
        body: JSON.stringify({ email, password })
      });

      if (response.ok) {
        const data = await response.json();
        console.log("Login successful:", data);
        navigate("/demo ");
      } else {
        const errorText = await response.text();
        console.error("Login failed:", response.status, errorText);
      }

    } catch (error) {
      console.error("Login failed:", error);
    }
  }

  return (
    <div className="login-panel">
      <div className="login-card">
        <h2 className="login-title">Zaloguj się</h2>
        <p className="login-subtitle">
          Wróć do planów lub odkryj spokojne miejsca w Twoim tempie.
        </p>
        
        <div className="social-login">
          <button className="social-btn google-btn">
            <img src={googleIcon} alt="Google Icon" className="social-icon" />
            Kontynuuj z Google
          </button>
          <button className="social-btn facebook-btn">
            <img src={facebookIcon} alt="Facebook Icon" className="social-icon" />
            Kontynuuj z Facebook
          </button>
          <button className="social-btn github-btn">
            <img src={githubIcon} alt="GitHub Icon" className="social-icon" />
            Kontynuuj z GitHub
          </button>
        </div>
        
        <div className="separator">
          <span className="separator-text">albo</span>
        </div>
        
        <div className="form-fields">
          <div className="input-group">
            <label className="input-label">Email</label>
            <input 
              type="email" 
              className="form-input" 
              placeholder="np. anna@calm.dev"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>
          
          <div className="input-group">
            <label className="input-label">Hasło</label>
            <input 
              type="password" 
              className="form-input" 
              placeholder="••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>
        </div>

        <button className="login-btn" onClick={handleLogin}>
          Zaloguj się
        </button>

        <div className="register-link">
          <span className="register-text">Nie masz konta? </span>
          <a className="register-text-link" onClick={() => navigate("/register")}>
            Zarejestruj się
          </a>
        </div>
      </div>
    </div>
  );
};

export default LoginPanel;
