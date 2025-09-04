import React from "react";
import "./Navbar.css";

const Navbar = () => (
  <nav className="navbar">
    <a href="#features" className="navbar-link">Funkcje</a>
    <a href="#how" className="navbar-link">Jak to działa</a>
    <a href="#places" className="navbar-link">Miejsca</a>
    <a href="#pricing" className="navbar-link">Cennik</a>
    <a href="#contact" className="navbar-link">Kontakt</a>
  </nav>
);

export default Navbar;
