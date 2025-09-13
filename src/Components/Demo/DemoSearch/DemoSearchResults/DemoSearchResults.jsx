import "./DemoSearchResults.css"

const DemoSearchResults = ({ results, activeIndex, setActiveIndex, onSave }) => {
    return (
        <div className="demo-search-results">
            {results.map((result, id) => (
                <div
                    key={id}
                    className={`result-item ${activeIndex === id ? "active" : ""}`}
                    onMouseEnter={() => setActiveIndex(id)}
                    onClick={() => onSave(result)}
                >
                    {result}
                </div>
            ))}
        </div>
    )
}

export default DemoSearchResults
