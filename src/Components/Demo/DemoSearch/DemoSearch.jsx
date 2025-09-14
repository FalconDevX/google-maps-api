import DemoHeader from '../DemoHeader/DemoHeader'
import "./DemoSearch.css"
import DemoSearchBar from './DemoSearchBar/DemoSearchBar'
import { useState, useRef } from 'react'
import DemoSearchResults from './DemoSearchResults/DemoSearchResults'
import { SaveIcon, XIcon } from 'lucide-react'

const DemoSearch = () => {
    const [results, setResults] = useState([]);
    const [activeIndex, setActiveIndex] = useState(-1);
    const [savedLocations, setSavedLocations] = useState([]);
    const [limitMessage, setLimitMessage] = useState("");
    const timeoutRef = useRef(null);  

    const handleSaveLocation = (location) => {
        if (savedLocations.length >= 10) {
            if (timeoutRef.current) {
                clearTimeout(timeoutRef.current);
                timeoutRef.current = null;
            }

            setLimitMessage("");
            
            setTimeout(() => {
                setLimitMessage("Osiągnąłeś limit 10 zapisanych lokalizacji");
                timeoutRef.current = setTimeout(() => {
                    setLimitMessage("");
                    timeoutRef.current = null;
                }, 3000);
            }, 10);
            
            return;
        }

        if (!savedLocations.includes(location)) {
            setSavedLocations([...savedLocations, location]);
        }
    };

    return (
        <>
            {limitMessage && (
                <div className="demo-search-limit-toast">
                    {limitMessage}
                </div>
            )}

            <div className="demo-search">           
                <DemoHeader />
                <div className="demo-search-layout">
                    <div className="demo-search-left">
                    </div>
                    <div className="demo-search-content">
                        <DemoSearchBar 
                            results={results} 
                            setResults={setResults} 
                            activeIndex={activeIndex} 
                            setActiveIndex={setActiveIndex} 
                            onSave={handleSaveLocation}
                        />
                        <DemoSearchResults 
                            results={results} 
                            activeIndex={activeIndex} 
                            setActiveIndex={setActiveIndex} 
                            onSave={handleSaveLocation} 
                        />
                    </div>

                    <div className="demo-search-right">
                        <div className="demo-search-saved-locations">
                            <div className="demo-search-saved-locations-header">
                                <SaveIcon size={18} strokeWidth={1.2} color='#ffffff'/>
                                <h2>Zapisane wyszukiwania</h2>
                            </div>
                            {savedLocations.length === 0 ? (
                                <h4>Na razie pusto. Wkrótce dostępne będą zapisane wyszukiwania.</h4>
                            ) : (
                                <ul className="demo-search-saved-locations-list">
                                    {savedLocations.map((loc, idx) => (
                                        <div className="demo-search-saved-location" key={idx}>
                                            {loc} 
                                            <button 
                                                className="demo-search-saved-location-remove-btn" 
                                                onClick={() => {
                                                    setSavedLocations(savedLocations.filter((_, i) => i !== idx));
                                                }}
                                            >
                                                <XIcon size={15} strokeWidth={2} color='#ffffff'/>
                                            </button>
                                        </div>
                                    ))}
                                </ul>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </>
    )
}

export default DemoSearch
