import DemoHeader from "./DemoHeader/DemoHeader";
import "./Demo.css";
import DemoCard from "./DemoCard/DemoCard";

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
      </div>
    </div>
  );
};

export default Demo;


