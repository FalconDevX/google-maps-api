import Navbar from "../Navbar/Navbar"
import logo from "../../../assets/social-icons/logo.png"
import  "./Header.css"

const Header = () => {
    return (
        <div className="header">
            <img src={logo} alt="Logo" className="logo"/>
            <Navbar />
            <div className="header-right">
                <button className="header-btn demo-btn">Demo</button>
                <button className="header-btn start-btn">Zacznij</button>
            </div>
        </div>
    )
}

export default Header