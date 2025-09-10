import "./DemoSearchResults.css"

const DemoSearchResults = ({ results, activeIndex, setActiveIndex }) => {
    return (
        <div className="demo-search-results">
            {results.map((result, id) => (
                <div
                    key={id}
                    className={`result-item ${activeIndex === id ? "active" : ""}`}
                    onClick={() => console.log("Kliknięto:", result)}
                    onMouseEnter={() => setActiveIndex(id)}
                >
                    {result}
                </div>
            ))}
        </div>
    )
}

export default DemoSearchResults
