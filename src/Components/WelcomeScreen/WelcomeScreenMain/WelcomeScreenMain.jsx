import Header from "../Header/Header"
import LoginPanel from "../LoginPanel/LoginPanel"
import "./WelcomeScreenMain.css"

const WelcomeScreenMain = () => {
  return (
    <div className='welcome-screen-main'>
        <Header />
            <div className="main-content">
                <div className="left-section">
                    <h1 className="main-heading">
                        <span className="heading-line">Spokojne podróże,</span>
                        <span className="heading-line-green">mądre planowanie,</span>
                        <span className="heading-line">trasy przyjazne</span>
                        <span className="heading-line-green">naturze</span><span>.</span>
                    </h1>
                    <p className="main-description">
                    Odnajdź zakątki, w których czas płynie wolniej. Ciche ścieżki, otoczone zielenią i naturą, gdzie odpoczniesz od zgiełku miasta. Nasz algorytm dopasuje miejsca do Twoich upodobań, dbając jednocześnie o Twój spokój i mniejszy ślad węglowy.
                    </p>
                </div>
                <LoginPanel />  
        </div>
    </div>
  )
}

export default WelcomeScreenMain