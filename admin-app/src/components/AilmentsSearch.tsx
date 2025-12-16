import React from 'react';

interface AilmentsSearchProps {
  searchTerm: string;
  onChange: (value: string) => void;
}

const AilmentsSearch: React.FC<AilmentsSearchProps> = ({ searchTerm, onChange }) => {
  return (
    <div style={{ marginBottom: '24px' }}>
      <input
        type="text"
        placeholder="Search by Name, Physical Connections, or Tags"
        value={searchTerm}
        onChange={(e) => onChange(e.target.value)}
        style={{
          width: '100%',
          padding: '12px 16px',
          fontSize: '14px',
          border: '1px solid #d0d0d0',
          borderRadius: '6px',
          boxSizing: 'border-box',
          fontFamily: 'inherit',
          transition: 'border-color 0.2s, box-shadow 0.2s',
          outline: 'none',
        }}
        onFocus={(e) => {
          e.currentTarget.style.borderColor = '#0066cc';
          e.currentTarget.style.boxShadow = '0 0 0 3px rgba(0, 102, 204, 0.1)';
        }}
        onBlur={(e) => {
          e.currentTarget.style.borderColor = '#d0d0d0';
          e.currentTarget.style.boxShadow = 'none';
        }}
      />
    </div>
  );
};

export default AilmentsSearch;
