import React from 'react'
import DemoHeader from '../DemoHeader/DemoHeader'
import { Search } from 'lucide-react'
import "./DemoSearch.css"
import Demo from '../Demo'
import DemoSearchBar from './DemoSearchBar/DemoSearchBar'
import { useState } from 'react'
import DemoSearchResults from './DemoSearchResults/DemoSearchResults'

const DemoSearch = () => {
    const [results, setResults] = useState([]);
    const [activeIndex, setActiveIndex] = useState(-1);

    return (
        <div className="demo-search">
            <DemoHeader />
            <div className="demo-search-content">
                <DemoSearchBar results={results} setResults={setResults} activeIndex={activeIndex} setActiveIndex={setActiveIndex} />
                <DemoSearchResults results={results} activeIndex={activeIndex} setActiveIndex={setActiveIndex} />
            </div>
        </div>
  )
}

export default DemoSearch