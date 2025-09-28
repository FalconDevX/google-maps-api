import DemoHeader from "./DemoHeader/DemoHeader";
import "./Demo.css";
import DemoCard from "./DemoCard/DemoCard";
import {BookMarkedIcon,SettingsIcon,ChartNoAxesCombined} from "lucide-react";
import { useState, useEffect } from "react";
import morskieOko from "../../assets/pictures/morskie-oko.png";
import poloninaWetlinska from "../../assets/pictures/polonina-wetlinska.png";
import sniardwy from "../../assets/pictures/sniardwy.png";
import DemoLeftSectionTitle from "./DemoLeftSectionTitle/DemoLeftSectionTitle";
import apiFetch from "../../../apifetch.js";


const Demo = () => {
  const places = [
    {
      location: "Tatry",
      country: "PL Polska",
      title: "Morskie Oko • Tatry",
      description:
        "Górskie jezioro otoczone świerkami. Szlaki o niskim natężeniu ruchu poza sezonem.",
      tags: ["jezioro", "góry", "cisza"],
      image: morskieOko
    },
    {
      location: "Bieszczady",
      country: "PL Polska",
      title: "Połonina Wetlińska • Bieszczady",
      description:
        "Rozległe łąki z widokiem na góry. Spokojne szlaki i mały ruch turystyczny.",
      tags: ["góry", "łąki", "spokój"],
      image: poloninaWetlinska
    },
    {
      location: "Mazury",
      country: "PL Polska",
      title: "Jezioro Śniardwy • Mazury",
      description:
        "Największe jezioro w Polsce, idealne do żeglowania i kajakarstwa. Spokojne plaże i lasy.",
      tags: ["jezioro", "żeglowanie", "spokój"],
      image: sniardwy
    },
  ];

  const [currentReelIndex, setCurrentReelIndex] = useState(0);
  const [savedPlaces, setSavedPlaces] = useState([]);

  const handleSavePlace = async (place) => {
    if (!savedPlaces.some((p) => p.title === place.title)) {
      setSavedPlaces([...savedPlaces, place]);
      try {
        await uploadSavedPlace(place);
        console.log("✅ Place saved successfully");
      } catch (err) {
        console.warn("❌ Save failed:", err.message);
        alert(err.message);
      }
    }
  };

  const uploadSavedPlace = async (place) => {
    const response = await apiFetch("/api/UserPlaces/addNewPlace", {
      method: "POST",
      body: JSON.stringify(place)
    });
      if (!response.ok) {
        console.error("Failed to save place:", await response.text());
      }
      else{
        console.log("Place saved successfully");
      }
  };

  const onSwipeUp = () => {
    setCurrentReelIndex((prevIndex) => (prevIndex + 1) % places.length);
  };

  const onSwipeDown = () => {
    handleSavePlace(places[currentReelIndex]);
    setCurrentReelIndex((prevIndex) => (prevIndex + 1) % places.length);
  };

  useEffect(() => {
    const fetchSavedPlaces = async () => {
      try {
        const response = await apiFetch("/api/UserPlaces/getPlaces", { method: "GET" });

        if (!response.ok) {
          console.warn("⚠️ Błąd pobierania miejsc:", await response.text());
          return;
        }

        const data = await response.json();
        console.log("📦 Pobrane miejsca:", data.places);

        setSavedPlaces(data.places || []);
      } catch (error) {
        console.error("❌ Błąd połączenia:", error);
      }
    };

    fetchSavedPlaces();
  }, []);


  return (
    <div className="demo-page">
      <DemoHeader />
      <div className="demo-content">
          <div className="demo-left-section">
            <DemoLeftSectionTitle />
          </div>

        <DemoCard
          location={places[currentReelIndex].location}
          country={places[currentReelIndex].country}
          title={places[currentReelIndex].title}
          description={places[currentReelIndex].description}
          tags={places[currentReelIndex].tags}
          image={places[currentReelIndex].image}
          onSwipeUp={onSwipeUp}
          onSwipeDown={onSwipeDown}
        />

        <div className="demo-right-section">
          <div className="panel-card">
            <div className="header-row">
              <BookMarkedIcon size={18} strokeWidth={1.2} />
              <h2>Zapisane miejsca</h2>            
              
            </div>
          
            {savedPlaces.length === 0 ? (
              <h4>Na razie pusto. Przeciągnij kartę w dół, aby zapisać.</h4>
              ) : (
                <ul className="demo-saved-places-list">
                  {savedPlaces.map((place, index) => (
                    <div className="demo-saved-place" key={index}>
                      <div className="saved-image-wrapper">
                        <img src={place.image} alt={place.title} />
                        <span className="saved-title">{place.title}</span>
                      </div>
                    </div>
                  ))}
                </ul>
              )}
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
            <h4>
              Na razie pusto. Wkrótce dostępne będą statystyki użytkowania.
            </h4>
          </div>
        </div>
        
      </div>
    </div>
  );
};

export default Demo;
