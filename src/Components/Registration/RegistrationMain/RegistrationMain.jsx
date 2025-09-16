import RegistrationPanel from "../RegistrationPanel/RegistrationPanel"
import RegistrationHeader from "../RegistrationHeader/RegistrationHeader";

const RegistrationMain = () => {
  return (
    <div className='registration-screen-main'>
        <RegistrationHeader /> 
        <RegistrationPanel />  
    </div>
  )
}
export default RegistrationMain;