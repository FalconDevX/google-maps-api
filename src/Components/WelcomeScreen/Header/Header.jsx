import Navbar from "../Navbar/Navbar"
import logo from "../../../assets/social-icons/logo.png"
import  "./Header.css"
import { useNavigate } from "react-router-dom"

const Header = () => {
    const navigate = useNavigate();
    return (
        <div className="header">
            <img src={logo} alt="Logo" className="logo"/>
            <Navbar />
            <div className="header-right">
                <button className="header-btn demo-btn" onClick={() => navigate('/demo')}>Demo</button>
                <button className="header-btn start-btn">Zacznij</button>
            </div>
        </div>
    )
}

export default Header