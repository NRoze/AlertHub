import React from 'react';

interface LocationSearchProps {
  searchTerm: string;
  setSearchTerm: (val: string) => void;
  suggestions: any[];
  onSelect: (loc: any) => void;
}

export const LocationSearch: React.FC<LocationSearchProps> = ({ 
  searchTerm, setSearchTerm, suggestions, onSelect 
}) => {
  return (
    <div className="sidebar-monitor__search-container">
      <input
        type="text"
        className="sidebar-monitor__search-input"
        placeholder="Search city..."
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
      />
      {suggestions.length > 0 && (
        <ul className="sidebar-monitor__suggestions">
          {suggestions.map(loc => (
            <li key={loc.id} onClick={() => onSelect(loc)}>
              <span>{loc.name}</span>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
};