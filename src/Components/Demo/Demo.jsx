import DemoHeader from "./DemoHeader/DemoHeader";
import "./Demo.css";
import DemoCard from "./DemoCard/DemoCard";
import {BookMarkedIcon,SettingsIcon,ChartNoAxesCombined} from "lucide-react";
import { useState } from "react";
import morskieOko from "../../assets/pictures/morskie-oko.png";
import poloninaWetlinska from "../../assets/pictures/polonina-wetlinska.png";
import sniardwy from "../../assets/pictures/sniardwy.png";

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

  const onSwipeUp = () => {
    setCurrentReelIndex((prevIndex) => (prevIndex + 1) % places.length);
  };

  return (
    <div className="demo-page">
      <DemoHeader />
      <div className="demo-content">
        <DemoCard
          location={places[currentReelIndex].location}
          country={places[currentReelIndex].country}
          title={places[currentReelIndex].title}
          description={places[currentReelIndex].description}
          tags={places[currentReelIndex].tags}
          image={places[currentReelIndex].image}
          onSwipeUp={onSwipeUp}
        />

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
