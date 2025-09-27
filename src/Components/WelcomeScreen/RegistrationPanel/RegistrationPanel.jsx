import React, { useState } from "react";
import { useNavigate } from "react-router-dom"
import "./RegistrationPanel.css"

const RegistrationPanel = () => {
  const navigate = useNavigate();

  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');

    if (password !== confirmPassword) {
      setError('Hasła nie są takie same!');
      return;
    }

    const registrationData = {
      username: username,
      email: email,
      password: password,
    };

    try {
      const response = await fetch('http://34.56.66.163/api/Users/register', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(registrationData),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Rejestracja nie powiodła się.');
      }
      
      setUsername('');
      setEmail('');
      setPassword('');
      setConfirmPassword('');

    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="registration-panel">
      <form className="registration-card" onSubmit={handleSubmit}>
        <h2 className="registration-title">Utwórz konto</h2>
        <div className="form-fields">
          <div className="input-group">
            <label className="input-label">Username</label>
            <input
              type="text"
              className="form-input"
              placeholder="np. XYZ"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </div>

          <div className="input-group">
            <label className="input-label">Email</label>
            <input
              type="email"
              className="form-input"
              placeholder="np. abc@xyz.dev"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
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
              required
            />
          </div>

          <div className="input-group">
            <label className="input-label">Potwierdź hasło</label>
            <input
              type="password"
              className="form-input"
              placeholder="••••••••"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              required
            />
          </div>
        </div>

        {error && <p className="error-message">{error}</p>}
                
        <button type="submit" className="login-btn">Zarejestruj się</button>

        <div className="login-link">
          <span className="login-text">Masz już konto? </span>
          <a className="login-text-link" onClick={() => navigate("/")}>
            Zaloguj się
          </a>
        </div>
      </form>
    </div>
  );
};

export default RegistrationPanel;