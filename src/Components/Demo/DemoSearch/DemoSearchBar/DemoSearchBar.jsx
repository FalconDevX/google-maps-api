import { Search } from 'lucide-react'
import "./DemoSearchBar.css"    
import { useState, useRef } from 'react'

const DemoSearchBar = ({ results, setResults, activeIndex, setActiveIndex, onSave }) => {
    const [input, setInput] = useState("");
    const abortController = useRef(null);

    const fetchData = (query) => {
        if (!query || query.trim() === "") {
            setResults([]);
            if (abortController.current) {
                abortController.current.abort();
            }
            return;
        }

        if (abortController.current) {
            abortController.current.abort();
        }

        abortController.current = new AbortController();

        const user = JSON.parse(localStorage.getItem("user"));
        const token = user?.accessToken;

        if (!token) {
            console.error("Brak tokenu autoryzacji");
            setResults([]);
            return;
        }

        const url = `http://34.56.66.163/api/GoogleMaps/getAutocompletePredictions?query=${encodeURIComponent(query)}`;
        fetch(url, { 
            signal: abortController.current.signal,
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(data => {
                if (!query || query.trim() === "") {
                    setResults([]);
                    return;
                }
                // Sprawdź czy data jest tablicą przed wywołaniem map()
                if (Array.isArray(data)) {
                    const results = data.map(item => item.placePrediction.text.text);
                    setResults(results);
                } else {
                    console.warn("API zwróciło nieoczekiwane dane:", data);
                    setResults([]);
                }
            })
            .catch(err => {
                if (err.name === "AbortError") return; 
                console.error("Błąd połączenia:", err);
            });
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
            const selected = results[activeIndex];
            setInput("");
            setResults([]);
            setActiveIndex(-1);

            if (onSave)
            {
                onSave(selected);
            }

        } else if (e.key === "Escape") {
            setResults([]);
            setActiveIndex(-1);
        }
    };

    return (
        <div className="demo-search-bar">
            <Search className="demo-search-icon" />
            <input 
                placeholder='Type to search place...' 
                value={input} 
                onChange={(e) => handleSearch(e.target.value)}  
                onKeyDown={handleKeyDown}
            />
        </div>
    )
}

export default DemoSearchBar
