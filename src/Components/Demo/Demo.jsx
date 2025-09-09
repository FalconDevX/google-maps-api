import DemoHeader from "./DemoHeader/DemoHeader"; 
import "./Demo.css"; 
import DemoCard from "./DemoCard/DemoCard"; 
import { BookMarkedIcon, SettingsIcon, ChartNoAxesCombined } from "lucide-react";  

const Demo = () => {   
  const sampleData = {     
    location: "Tatry",     
    country: "PL Polska",      
    title: "Morskie Oko • Tatry",     
    description: "Górskie jezioro otoczone świerkami. Szlaki o niskim natężeniu ruchu poza sezonem.",     
    tags: ["jezioro", "góry", "cisza"],     
    image: "https://images.unsplash.com/photo-1506905925346-14b1e587d4d3?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80"   
  };    

  return (     
    <div className="demo-page">       
      <DemoHeader />       
      <div className="demo-content">         
        <DemoCard {...sampleData} />         
        <div className="demo-right-section">           
          <div className="panel-card">             
            <div className="header-row">               
              <BookMarkedIcon size={18} strokeWidth={1.2} />               
              <h2>Zapisane lokalizacje</h2>             
            </div>             
            <h4>Na razie pusto. Przeciągnij kartę w dół, aby zapisać.</h4>           
          </div>            

          <div className="panel-card">             
            <div className="header-row">               
              <SettingsIcon size={18} strokeWidth={1.2} />               
              <h2>Filtry i ustawienia</h2>             
            </div>             
            <h4>Na razie pusto. Wkrótce dostępne będą opcje dostosowywania.</h4>           
          </div>            

          <div className="panel-card">             
            <div className="header-row">               
              <ChartNoAxesCombined size={18} strokeWidth={1.2} />               
              <h2>Statystyki</h2>             
            </div>             
            <h4>Na razie pusto. Wkrótce dostępne będą statystyki użytkowania.</h4>           
          </div>         
        </div>       
      </div>     
    </div>   
  ); 
};  

export default Demo;