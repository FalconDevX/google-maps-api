import { Search } from 'lucide-react'
import "./DemoSearchBar.css"    
import { useState } from 'react'

const DemoSearchBar = ({ results, setResults, activeIndex, setActiveIndex }) => {
    const [input, setInput] = useState("");

    const fetchData = (query) => {
        if (!query || query.trim() === "") {
            
            setResults([]);
            return;
        }

        const url = `http://34.56.66.163/api/GoogleMaps/getAutocompletePredictions?query=${encodeURIComponent(query)}`;
        fetch(url)
            .then(response => response.json())
            .then(data => {
                const results = data.map(item => item.placePrediction.text.text);
                setResults(results);
            })
            .catch(err => console.error("Błąd połączenia:", err));
    };

    const handleSearch = (query) => {
        setInput(query);
        fetchData(query);
    };

    const handleKeyDown = (e) => {
        if (results.length === 0) return;

        if (e.key === "ArrowDown") {
            e.preventDefault(); 
            setActiveIndex((prev) => {
                const nextIndex = prev + 1;
                return nextIndex >= results.length ? 0 : nextIndex;
            });
        } else if (e.key === "ArrowUp") {
            e.preventDefault();
            setActiveIndex((prev) => {
                const nextIndex = prev - 1;
                return nextIndex < 0 ? results.length - 1 : nextIndex;
            });
        } else if (e.key === "Enter" && activeIndex >= 0) {
            e.preventDefault();
            setInput(results[activeIndex]);
            setResults([]);
            setActiveIndex(-1); 
        } else if (e.key === "Escape") {
            setResults([]);
            setActiveIndex(-1);
        }
    };

    return (
        <div className="demo-search-bar">
            <Search className="demo-search-icon" />
            <input placeholder='Type to search place...' value={input} onChange={(e) => handleSearch(e.target.value)}  onKeyDown={handleKeyDown}/>
        </div>
    )
}

export default DemoSearchBar